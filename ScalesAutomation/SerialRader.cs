using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using log4net;
using System.Reflection;

namespace ScalesAutomation
{
    public class MySerialReader : IDisposable
    {
        private SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MySerialReader()
        {
            serialPort = new SerialPort("COM5", 4800, Parity.Even, 7, StopBits.Two);
            serialPort.Open();

            serialPort.DataReceived += serialPort_DataReceived;
        }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];
            log.Debug("Bytes To Read: " + serialPort.BytesToRead.ToString() + Environment.NewLine);

            serialPort.Read(data, 0, data.Length);

            data.ToList().ForEach(b => recievedData.Enqueue(b));

            processData();
        }

        void processData()
        {
            log.Debug("Process Data..." + System.Environment.NewLine);

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
            if (recievedData.Count >= 15)
            {
                var packet = Enumerable.Range(0, 15).Select(i => recievedData.Dequeue());
                log.Debug(packet.ToArray());
                recievedData.Clear();
            }
        }

        public void Dispose()
        {
            if (serialPort != null)
                serialPort.Dispose();
        }
    }
}
