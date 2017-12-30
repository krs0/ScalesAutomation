using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public class MySerialReader : IDisposable
    {
        private SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private struct measurement
        {
            public bool stable;
            public int weight;
        }

        private List<measurement> measurements = new List<measurement>();

        public MySerialReader()
        {
            serialPort = new SerialPort("COM5", 4800, Parity.Even, 7, StopBits.Two);
            Thread.Sleep(1000);

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                serialPort.DataReceived += serialPort_DataReceived;
            }
        }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];
            // log.Debug("Bytes To Read: " + serialPort.BytesToRead.ToString() + Environment.NewLine);

            serialPort.Read(data, 0, data.Length);

            data.ToList().ForEach(b => recievedData.Enqueue(b));

            processData();
        }

        void processData()
        {
            measurement oneMeasurement = new measurement();
            // Delete from receivedData Until a Start Character is found
            var queueCount = recievedData.Count;
            for (var i = 0; i < queueCount; i++)
            {
                if (recievedData.ElementAt(0) != 0x24)
                    recievedData.Dequeue();
                else
                    break;
            }
            
            // Determine if we have a "packet" in the queue
            if (recievedData.Count >= 8)
            {
                var packet = Enumerable.Range(0, 8).Select(i => recievedData.Dequeue());
                var packetArray = packet.ToArray();
                measurements.Add(oneMeasurement);

                var charArray = Array.ConvertAll(packetArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
                var intArray = Array.ConvertAll(charArray, element => (int)char.GetNumericValue(element));


                oneMeasurement.stable = (intArray[1] == 0 ? true : false);
                oneMeasurement.weight = intArray[6] + intArray[5] * 10 + intArray[4] * 100 + intArray[3] * 1000 + intArray[2] * 10000;

                log.Debug(packetArray + Environment.NewLine);
                log.Debug("Stable: " + oneMeasurement.stable + " - Weight: " + oneMeasurement.weight + Environment.NewLine);
                recievedData.Clear();
            }
        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.Dispose();
            }
        }
    }
}
