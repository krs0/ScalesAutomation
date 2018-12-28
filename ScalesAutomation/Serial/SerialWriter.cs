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
            byte[][] dataToTransmit = LoadSimulatedMeasurementsFromFile();
            int i = 0;

            for (i = 0; i < dataToTransmit.Length; i++)
            {
                WriteData(dataToTransmit[i]);
                Thread.Sleep(20); // in reality is 100ms for ustable measurement and 200ms for stable measurement
            }
        }

        /// <summary>This function will load a series of measurements from a file. It can work with real logs provided by this program</summary>
        /// <returns>A 2 dimensional byte array where each entry is one measurement (an array of bytes)</returns>
        public byte[][] LoadSimulatedMeasurementsFromFile()
        {
            string simulatedFilePath = Settings.Default.SerialTransmissionSimulationPath;
            string line;
            string measurement;
            string[] lineAsStringArray;
            int i = 0;
            byte stable = 0;
            byte wrappedMeasurementLength = 8;

            var lineCount = File.ReadLines(simulatedFilePath).Count();
            byte[][] byteArray2d = new byte[lineCount][]; // give it max possible size

            log.Info("Start Loading Simulated Data from: " + simulatedFilePath);

            using (var file = new StreamReader(simulatedFilePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    // if measurement data found in the line, add it to output array
                    // Line format supported: 2018-09-05 07:07:16,950 INFO  S: F - W: 140
                    var startOfMeasurement = line.IndexOf("- W: ") + 5; // Start of wanted pattern + pattern length
                    var startOfStable = line.IndexOf(" S: ") + 4; // Start of wanted pattern + pattern length

                    if (startOfMeasurement != 4) // (-1 + length of "- W: ") ignore lines without measurements
                    {
                        byte[] lineAsByteArray = { 0x24, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x0D };

                        // Add stability information
                        var stableBooleanString = line.Substring(startOfStable, 1);
                        if (stableBooleanString == "F")
                            lineAsByteArray[1] = 0x31; // else it is already 0x30
                        
                        // Add measurement information
                        measurement = line.Substring(startOfMeasurement, line.Length - startOfMeasurement); // get only the measurement value
                        char[] charArray = measurement.ToCharArray();
                        Array.Reverse(charArray);
                        for (var j=0; j < charArray.Length; j++)
                            lineAsByteArray[wrappedMeasurementLength - 2 - j] = Convert.ToByte(charArray[j]);

                        // Add information from current line to returned array
                        byteArray2d[i] = new byte[wrappedMeasurementLength];
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
            // log.Info("Writing bytes: " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

    }
}
