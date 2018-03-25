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

            StartTransmissionFromFile();

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

        private void StartTransmissionFromFile()
        {
            byte[][] dataToTransmit = LoadSimulatedDataFromFile();
            int i = 0;

            for (i = 0; i < dataToTransmit.Length; i++)
            {
                WriteData(dataToTransmit[i]);
                Thread.Sleep(10); // in reality is 100ms for ustable measurement and 200ms for stable measurement
            }
        }

        /// <summary>This function will load a series of measurements from a file. It can work with real logs provided by this program</summary>
        /// <returns>A 2 dimensional byte array where each entry is one measurement (an array of bytes)</returns>
        public byte[][] LoadSimulatedDataFromFile()
        {
            string simulatedFilePath = Settings.Default.SerialTransmissionSimulationPath;
            string line;
            string strippedLine;
            string[] lineAsStringArray;
            int i = 0;

            var lineCount = File.ReadLines(simulatedFilePath).Count();
            byte[][] byteArray2d = new byte[lineCount][]; // give it max possible size

            log.Info("Start Loading Simulated Data from: " + simulatedFilePath);

            using (var file = new StreamReader(simulatedFilePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    // if measurement data found in the line, added to output array
                    var startOfMeasurement = line.IndexOf(" 24-3");
                    if (startOfMeasurement != -1)
                    {
                        strippedLine = line.Substring(startOfMeasurement + 1, 23); // get rid of starting SPACE char

                        lineAsStringArray = strippedLine.Split('-');
                        byte[] lineAsByteArray = lineAsStringArray.Select(s => Convert.ToByte(s, 16)).ToArray();
                        byteArray2d[i] = new byte[lineAsByteArray.Length];
                        lineAsByteArray.CopyTo(byteArray2d[i], 0);
                        i++;
                    }

                }
            }

            Array.Resize(ref byteArray2d, i); // correct the size because we had non-measurement lines

            log.Info("Loaded " + i + " measurements");
            log.Info("Stopped Loading Simulated Data" + Environment.NewLine);

            return byteArray2d;

        }

        public void WriteData(byte[] txBuffer)
        {
            // log.Debug("Writing bytes: " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

    }
}
