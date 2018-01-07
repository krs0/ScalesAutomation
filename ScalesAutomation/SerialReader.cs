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
        public SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();
        private bool alreadyAddedToList;
        private int lastMeasurement;

        public SynchronizedCollection<Measurement> Measurements;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MySerialReader(SynchronizedCollection<Measurement> measurements)
        {
            Measurements = measurements;

            serialPort = new SerialPort("COM5", 4800, Parity.Even, 7, StopBits.Two);

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                enableCyclicTransmission();
                serialPort.DataReceived += serialPort_DataReceived;
            }
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
            var oneMeasurement = new Measurement();
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

                var charArray = Array.ConvertAll(packetArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
                var intArray = Array.ConvertAll(charArray, element => (int)char.GetNumericValue(element));

                oneMeasurement.isStable = (intArray[1] == 0 ? true : false);
                oneMeasurement.weight = intArray[6] + intArray[5] * 10 + intArray[4] * 100 + intArray[3] * 1000 + intArray[2] * 10000;

                log.Debug("Package Received: " + BitConverter.ToString(packetArray) + Environment.NewLine);
                log.Debug("Stable: " + oneMeasurement.isStable + " - Weight: " + oneMeasurement.weight + Environment.NewLine);

                // addMeasurement()
                if (oneMeasurement.isStable)
                {
                    if (!alreadyAddedToList)
                    {
                        Measurements.Add(oneMeasurement);
                        lastMeasurement = oneMeasurement.weight;
                        alreadyAddedToList = true;
                    }
                    else
                    {
                        // TODO: Check for same value
                        if (lastMeasurement != oneMeasurement.weight)
                        {
                            log.Error("Measurement added to list does not match current measurement!" + Environment.NewLine);
                            Measurements.Add(oneMeasurement);
                            lastMeasurement = oneMeasurement.weight;
                            alreadyAddedToList = true;
                        }
                    }
                }
                else
                {
                    alreadyAddedToList = false;
                }

                recievedData.Clear();
                log.Debug("Measurements in list: " + Measurements.Count + Environment.NewLine);
            }
        }

        public void enableCyclicTransmission()
        {
            byte[] txBuffer = prepareCyclicTransmissionPackage();
            log.Debug("Enabling Cyclic Transmission... " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
            Thread.Sleep(10);
        }

        private byte[] prepareCyclicTransmissionPackage()
        {
            byte[] txBuffer = new byte[3];

            // assign to buffer
            txBuffer[0] = 0x73;
            txBuffer[1] = 0x78;
            txBuffer[2] = 0x0D; // CR = end character

            return txBuffer;

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
