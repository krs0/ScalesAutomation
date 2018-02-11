using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using log4net;
using System.Reflection;
using System.Threading;
using ScalesAutomation.Properties;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SerialPort serialPort;

        public MySerialWriter()
        {
            serialPort = new SerialPort(Settings.Default.WriteCOMPort, 4800, Parity.Even, 7, StopBits.Two);
            Thread.Sleep(100);

            if (!serialPort.IsOpen)
                serialPort.Open();

            startTransmissionFromFile();

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

        private void startTransmissionFromFile()
        {
            byte[][] dataToTransmit = loadSimulatedDataFromFile();
            int i = 0;

            for (i = 0; i < dataToTransmit.Length; i++)
            {
                writeData(dataToTransmit[i]);
                Thread.Sleep(10);
            }
        }

        public byte[][] loadSimulatedDataFromFile()
        {
            string line;
            string[] lineAsStringArray;
            int i = 0;


            var lineCount = File.ReadLines(Settings.Default.SerialTransmissionSimulationPath).Count();
            byte[][] byteArray2d = new byte[lineCount][];

            log.Debug("Loading from file: " + Environment.NewLine);

            var file = new StreamReader(Settings.Default.SerialTransmissionSimulationPath);
            while ((line = file.ReadLine()) != null)
            {
                lineAsStringArray = line.Split('-');
                byte[] lineAsByteArray = lineAsStringArray.Select(s => Convert.ToByte(s, 16)).ToArray();
                byteArray2d[i] = new byte[lineAsByteArray.Length];
                lineAsByteArray.CopyTo(byteArray2d[i], 0);
                i++;
            }

            file.Close();

            return byteArray2d;

        }

        public void writeData(byte[] txBuffer)
        {
            log.Debug("Writing bytes: " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

    }
}
