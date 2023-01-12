using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using log4net;
using System.Reflection;
using System.Threading;
using ScalesAutomation.Properties;
using System.Drawing;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SerialPort serialPort;

        public MySerialWriter()
        {
            try
            {
                var scaleType = Settings.Default.ScaleType;

                if(scaleType == "Bilanciai")
                    serialPort = new SerialPort(Settings.Default.WriteCOMPortForSimulation, 4800, Parity.Even, 7, StopBits.Two);
                else if(scaleType == "Constalaris")
                    serialPort = new SerialPort(Settings.Default.WriteCOMPortForSimulation, 9600, Parity.None, 8, StopBits.One);
                else
                    log.Error("Model cantar incorect: " + scaleType + " - Modelele suportate sunt: Bilanciai sau Constalaris");

                Thread.Sleep(100);

                if(!serialPort.IsOpen)
                    serialPort.Open();

                StartTransmissionFromFile();

                this.Dispose();
            }
            catch(ThreadAbortException ex)
            {
                this.Dispose();

                // Clean-up code can go here.  
                // If there is no Finally clause, ThreadAbortException is  
                // re-thrown by the system at the end of the Catch clause.
            }

        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                serialPort.DtrEnable = false;
                serialPort.RtsEnable = false;

                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Close();
                }

                Thread.Sleep(200);

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
                Thread.Sleep(30); // in reality is 100ms for ustable measurement and 200ms for stable measurement
            }
        }

        /// <summary>This function will load a series of measurements from a file. It can work with real logs provided by this program</summary>
        /// <returns>A 2 dimensional byte array where each entry is one measurement (an array of bytes)</returns>
        public byte[][] LoadSimulatedMeasurementsFromFile()
        {
            string simulatedFilePath = Settings.Default.SerialTransmissionSimulationPath;
            string line;
            string measurement;
            int i = 0;

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
                        // recreate from our logs the exact string the Scales is sending
                        switch(Settings.Default.ScaleType)
                        {
                            case "Bilanciai":
                                byte[] lineAsByteArray = { 0x24, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x0D };
                                byte wrappedMeasurementLength = 8;

                                // Add stability information
                                var stableBooleanString = line.Substring(startOfStable, 1);
                                if(stableBooleanString == "F")
                                    lineAsByteArray[1] = 0x31; // else it is already 0x30

                                // Add measurement information
                                measurement = line.Substring(startOfMeasurement, line.Length - startOfMeasurement); // get only the measurement value
                                char[] charArray = measurement.ToCharArray();
                                Array.Reverse(charArray);
                                for(var j = 0; j < charArray.Length; j++)
                                    lineAsByteArray[wrappedMeasurementLength - 2 - j] = Convert.ToByte(charArray[j]);

                                // Add information from current line to returned array
                                byteArray2d[i] = new byte[wrappedMeasurementLength];
                                lineAsByteArray.CopyTo(byteArray2d[i], 0);
                                i++;
                                break;

                            case "Constalaris": //Convert from "S: T - W: 955" to "ST,GS:  0.000kg<13><10>"
                                byte[] lineAsByteArray2 = { 83, 84, 44, 71, 83, 58, 32, 32, 48, 46, 48, 48, 48, 107, 103, 13, 10 };
                                byte wrappedMeasurementLength2 = 17;

                                // Add stability information
                                var stableBooleanString2 = line.Substring(startOfStable, 1);
                                if(stableBooleanString2 == "F") // Stable is filled aready as default
                                {
                                    lineAsByteArray2[0] = 85;
                                    lineAsByteArray2[1] = 83;
                                }

                                // Add measurement information
                                measurement = line.Substring(startOfMeasurement, line.Length - startOfMeasurement); // get only the measurement value, its in grams, needs to be in kg
                                char[] charArray2 = measurement.ToCharArray();
                                Array.Reverse(charArray2);
                                // insert grams before Kg<13><10>
                                for(var j = 0; j < charArray2.Length; j++)
                                {
                                    if(j >= 3) // stop to insert . point
                                        break;

                                    lineAsByteArray2[wrappedMeasurementLength2 - 5 - j] = Convert.ToByte(charArray2[j]);
                                }

                                // continue with kgs if needed before .
                                for(var j = 3; j < charArray2.Length; j++)
                                    lineAsByteArray2[wrappedMeasurementLength2 - 6 - j] = Convert.ToByte(charArray2[j]);

                                // Add information from current line to returned array
                                byteArray2d[i] = new byte[wrappedMeasurementLength2];
                                lineAsByteArray2.CopyTo(byteArray2d[i], 0);
                                i++;

                                break;
                        }
                    }
                }
            }

            Array.Resize(ref byteArray2d, i); // correct the size because we had non-measurement lines

            log.Info(String.Format("Loaded {0} measurements.", i));
            log.Info("Finished Loading Simulated Data" + Environment.NewLine);

            return byteArray2d;

        }

        public void WriteData(byte[] txBuffer)
        {
            // log.Info("Writing bytes: " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

    }
}
