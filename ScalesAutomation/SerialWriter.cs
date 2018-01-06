using System;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        private bool simulationEnabled;

        private SerialPort serialPort;
        private byte state = (byte)State.notValid;
        private int measurementStableCounter;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MySerialWriter(bool simulationEnabled)
        {
            this.simulationEnabled = simulationEnabled;

            serialPort = new SerialPort("COM6", 4800, Parity.Even, 7, StopBits.Two);
            Thread.Sleep(1000);

            if (!serialPort.IsOpen)
                serialPort.Open();

            enableCyclicTransmission();

            if (simulationEnabled)
            {
                measurementStableCounter = 0;
                startTransmission();
            }

            serialPort.Close();
            this.Dispose();

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

        public void startTransmission()
        {
            for (var i = 0; i < 100; i++)
            {
                byte[] txBuffer = prepareNextPackageStandard();

                writeData(txBuffer);
                Thread.Sleep(10);
            }
        }

        private byte[] prepareNextPackageNetAndCode()
        {
            byte[] txBuffer = new byte[15];
            byte[] measurement = new byte[4];
            byte[] code = new byte[6] { 97, 98, 99, 100, 101, 102 }; //ASCII abcdef

            measurement[0] = prepareRandomPackage();

            // assign to buffer
            txBuffer[0] = 0x24; // $ = start
            txBuffer[1] = state; // 0 = stable, 1 = unstable, 3 = not valid
            txBuffer[2] = (byte)(measurement[0] + 0);
            txBuffer[3] = (byte)(measurement[0] + 1);
            txBuffer[4] = (byte)(measurement[0] + 2);
            txBuffer[5] = (byte)(measurement[0] + 3);
            txBuffer[6] = (byte)(measurement[0] + 4);
            txBuffer[7] = 0x20; // 20 = space
            txBuffer[8] = code[0]; // 6 code digits
            txBuffer[9] = code[1];
            txBuffer[10] = code[2];
            txBuffer[11] = code[3];
            txBuffer[12] = code[4];
            txBuffer[13] = code[5];
            txBuffer[14] = 0x0D; // CR = end character

            return txBuffer;

        }

        private byte[] prepareNextPackageStandard()
        {
            byte[] txBuffer = new byte[8];
            byte[] measurement = new byte[5];

            measurement[0] = prepareRandomPackage();

            // assign to buffer
            txBuffer[0] = 0x24; // $ = start
            txBuffer[1] = state; // 0 = stable, 1 = unstable, 3 = not valid
            txBuffer[2] = (byte)(measurement[0] + 0);
            txBuffer[3] = (byte)(measurement[0] + 1);
            txBuffer[4] = (byte)(measurement[0] + 2);
            txBuffer[5] = (byte)(measurement[0] + 3);
            txBuffer[6] = (byte)(measurement[0] + 4);
            txBuffer[7] = 0x0D; // CR = end character

            return txBuffer;

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

        public void writeData(byte[] txBuffer)
        {
            log.Debug("Writing bytes: " + txBuffer + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

        private byte prepareRandomPackage()
        {
            byte initialMeasurement;
            Random rand = new Random();


            // we return same measurement 5 times once is stable then 3 unstable
            if (measurementStableCounter == 0 || measurementStableCounter > 7)
            {
                state = Convert.ToByte(rand.Next(48, 51));
                if (state == (byte)State.stable)
                {
                    measurementStableCounter = 1;
                    initialMeasurement = 49; //1 ASCII
                }
                else
                {
                    measurementStableCounter = 0;
                    initialMeasurement = Convert.ToByte(rand.Next(49, 57)); // ASCII '1'-'9'
                }
            }
            else
            {
                measurementStableCounter++;
                initialMeasurement = 49; //1 ASCII
                if (measurementStableCounter > 4)
                {
                    state = (byte)State.unstable;
                    initialMeasurement = Convert.ToByte(rand.Next(49, 57)); // ASCII '1'-'9'
                }
            }

            return initialMeasurement;
        }

        void enableCyclicTransmission()
        {
            byte[] txBuffer = prepareCyclicTransmissionPackage();

            writeData(txBuffer);
            Thread.Sleep(10);
        }

    }
}
