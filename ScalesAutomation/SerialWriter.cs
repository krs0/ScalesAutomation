using System;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        private SerialPort serialPort;
        private byte state = (byte)State.notValid;
        private int measurementStableCounter;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MySerialWriter()
        {
            serialPort = new SerialPort("COM6", 4800, Parity.Even, 7, StopBits.Two);
            serialPort.Open();

            measurementStableCounter = 0;
            StartTransmission();
        }

        public void Dispose()
        {
            if (serialPort != null)
                serialPort.Dispose();
        }

        public void StartTransmission()
        {
            for (var i = 0; i < 100; i++)
            {
                byte[] txBuffer = PrepareNextPackage();

                writeData(txBuffer);
                Thread.Sleep(10);
            }

            serialPort.Close();
        }

        private byte[] PrepareNextPackage()
        {
            byte[] txBuffer = new byte[15];
            byte[] measurement = new byte[4];
            byte[] code = new byte[6] { 97, 98, 99, 100, 101, 102 }; //ASCII abcdef
                        
            Random rand = new Random();

            // we return same measurement 5 times once is stable then 3 unstable
            if (measurementStableCounter == 0 || measurementStableCounter > 7)
            {
                state = Convert.ToByte(rand.Next(0, 2));
                if (state == (byte)State.stable)
                {
                    measurementStableCounter = 1;
                    measurement[0] = 49; //1 ASCII
                }
                else
                {
                    measurementStableCounter = 0;
                    measurement[0] = Convert.ToByte(rand.Next(71, 76)); // ASCII G-K
                }
            }
            else
            {
                measurementStableCounter++;
                measurement[0] = 49; //1 ASCII
                if (measurementStableCounter >4)
                {
                    state = (byte)State.unstable;
                    measurement[0] = Convert.ToByte(rand.Next(71, 76)); // ASCII G-K
                }
            }


            // assign to buffer
            txBuffer[0] = 0x24; // $ = start
            txBuffer[1] = state; // 0 = stable, 1 = unstable, 3 = not valid
            txBuffer[2] = measurement[0];
            txBuffer[3] = (byte)(measurement[0] + 1);
            txBuffer[4] = (byte)(measurement[0] + 2); ;
            txBuffer[5] = (byte)(measurement[0] + 3); ;
            txBuffer[6] = 0x36; // weight unit
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

        public void writeData(byte[] txBuffer)
        {
            log.Debug(Environment.NewLine);
            log.Debug("Serial Transmission Started!" + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);

        }


    }
}
