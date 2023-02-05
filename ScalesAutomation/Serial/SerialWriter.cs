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
        private static readonly ILog log = LogManager.GetLogger("generalLog");
        private SerialPort serialPort;

        public MySerialWriter()
        {
            try
            {
                // set porperties for serial port
                switch(Settings.Default.ScaleType)
                {
                    case "Bilanciai":
                        serialPort = new SerialPort(Settings.Default.WriteCOMPortForSimulation, 4800, Parity.Even, 7, StopBits.Two);
                        break;
                    case "Constalaris":
                        serialPort = new SerialPort(Settings.Default.WriteCOMPortForSimulation, 9600, Parity.None, 8, StopBits.One);
                        break;
                    default:
                        log.Error($"Model cantar incorect: {Settings.Default.ScaleType} - Modelele suportate sunt: Bilanciai sau Constalaris");
                        break;
                }

                Thread.Sleep(100);

                serialPort.Open();

                LoadSimulatedMeasurementsFromFile(out byte[][] dataToTransmit);

                TransmitSimulatedMeasurements(dataToTransmit);
            }
            catch(Exception ex)
            {
                log.Info($"Error in Serial Writer! {ex.Message}");
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            SerialPortDispose();
        }

        /// <summary>
        /// Loads all measurements from a previously recorded log
        /// </summary>
        /// Log line format: 2018-09-05 07:07:16,950 INFO  S: F - W: 140
        /// Created measurement lines depend on scale type:
        /// ST,GS:  0.000kg<13><10> Constalaris format
        /// *********************** Bilanciai format
        /// Note: It ignores all lines that do not contain a measurement
        /// <returns>A 2 dimensional byte array where each entry is one measurement</returns>
        private void LoadSimulatedMeasurementsFromFile(out byte[][] simulatedMeasurements)
        {
            string line;
            int simulatedMeasurementsIndex = 0;
            bool noMeasurementLine;
            byte[] rawLogLine = { };
            int rawLogLineLength = 0;

            string simulatedFilePath = Settings.Default.SerialTransmissionSimulationPath;
            log.Info($"Starting Scales Simulation...{Environment.NewLine}\tLoading Simulated Data from: '{simulatedFilePath}'");

            var lineCount = File.ReadLines(simulatedFilePath).Count();
            simulatedMeasurements = new byte[lineCount][]; // max possible size (including non measurements lines)

            using (var file = new StreamReader(simulatedFilePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    var startOfStable = line.IndexOf(" S: ") + 4; // Start of wanted pattern + pattern length
                    var startOfMeasurement = line.IndexOf("- W: ") + 5; // Start of wanted pattern + pattern length
                    noMeasurementLine = startOfMeasurement == 4; // -1 (because not found) + length of "- W: "

                    if(noMeasurementLine)
                        continue;

                    // recreate from our logs the exact string the Scales is sending
                    switch(Settings.Default.ScaleType)
                    {
                    case "Bilanciai": //Convert from "S: T - W: 955" to "..."
                        CreateMeasurementLineBilanciai(line, startOfStable, startOfMeasurement, out rawLogLine, out rawLogLineLength);
                        break;

                    case "Constalaris": //Convert from "S: T - W: 955" to "ST,GS:  0.000kg<13><10>"
                        CreateMeasurementLineConstalaris(line, startOfStable, startOfMeasurement, out rawLogLine, out rawLogLineLength);
                        break;
                    }

                    AddRawLogLineToSimulatedMeasurements(simulatedMeasurements, ref simulatedMeasurementsIndex, rawLogLine, rawLogLineLength);
                }
            }

            Array.Resize(ref simulatedMeasurements, simulatedMeasurementsIndex); // correct the size because we had non-measurement lines

            log.Info($"Loaded {simulatedMeasurementsIndex} measurements");
            log.Info($"Finished Scales Simulation!");
        }

        private static void AddRawLogLineToSimulatedMeasurements(byte[][] simulatedMeasurements, ref int simulatedMeasurementsIndex, byte[] rawLogLine, int rawLogLineLength)
        {
            simulatedMeasurements[simulatedMeasurementsIndex] = new byte[rawLogLineLength];
            rawLogLine.CopyTo(simulatedMeasurements[simulatedMeasurementsIndex], 0);
            simulatedMeasurementsIndex++;
        }

        /// <summary>
        /// Transforms a Scales automation log line in Bilanciai raw format
        /// </summary>
        private static void CreateMeasurementLineBilanciai(string line, int startOfStable, int startOfMeasurement, out byte[] rawLogLine, out int rawLogLineLength)
        {
            rawLogLineLength = 8;
            rawLogLine = new byte[] { 0x24, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x0D };

            // Add stability information
            var stableBooleanString = line.Substring(startOfStable, 1);
            if(stableBooleanString == "F")
                rawLogLine[1] = 0x31; // 0x30 (T) was default

            // Add measurement information
            var measurement = line.Substring(startOfMeasurement, line.Length - startOfMeasurement); // get only the measurement value
            char[] measurementAsCharArray = measurement.ToCharArray();
            Array.Reverse(measurementAsCharArray);
            for(var j = 0; j < measurementAsCharArray.Length; j++)
                rawLogLine[rawLogLineLength - 2 - j] = Convert.ToByte(measurementAsCharArray[j]);
        }

        /// <summary>
        /// Transforms a Scales automation log line in Constalaris raw format
        /// </summary>
        private static void CreateMeasurementLineConstalaris(string line, int startOfStable, int startOfMeasurement, out byte[] rawLogLine, out int rawLogLineLength)
        {
            rawLogLineLength = 17;
            rawLogLine = new byte[] { 83, 84, 44, 71, 83, 58, 32, 32, 48, 46, 48, 48, 48, 107, 103, 13, 10 };

            // Add stability information
            var stableEntry = line.Substring(startOfStable, 1);
            if(stableEntry == "F") // put US in array instead of default ST
            {
                rawLogLine[0] = 85;
                rawLogLine[1] = 83;
            }

            // Add measurement information
            var measurement = line.Substring(startOfMeasurement, line.Length - startOfMeasurement); // get only the measurement value, its in grams, needs to be in kg
            char[] measurementAsCharArray = measurement.ToCharArray();
            Array.Reverse(measurementAsCharArray);
            // insert measurement value before Kg<13><10>. Keep in mind that for measurements > 1Kg a . exists!
            for(var j = 0; j < measurementAsCharArray.Length; j++)
            {
                if(j >= 3) // stop to insert . point
                    break;

                rawLogLine[rawLogLineLength - 5 - j] = Convert.ToByte(measurementAsCharArray[j]);
            }

            // continue with Kg digits if needed (before .)
            for(var j = 3; j < measurementAsCharArray.Length; j++)
                rawLogLine[rawLogLineLength - 6 - j] = Convert.ToByte(measurementAsCharArray[j]);
        }

        private void TransmitSimulatedMeasurements(byte[][] simulatedMeasurements)
        {
            for(int i = 0; i < simulatedMeasurements.Length; i++)
            {
                serialPort.Write(simulatedMeasurements[i], 0, simulatedMeasurements[i].Length);
                Thread.Sleep(10); // in reality is 100ms for ustable measurement and 200ms for stable measurement

                if(ScalesAutomationForm.stopTransmission) // stop this thread when stop lot is pressed in main
                    break;
            }
        }

        private void SerialPortDispose()
        {
            try
            {
                if(serialPort != null)
                {
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;

                    Thread.Sleep(200);

                    if(serialPort.IsOpen)
                    {
                        serialPort.DiscardInBuffer();
                        serialPort.DiscardOutBuffer();
                        serialPort.Close();
                    }

                    serialPort.Dispose();
                }
            }
            catch(Exception ex)
            {
                log.Error($"Cannot Dispose of serial port!{Environment.NewLine}{ex.Message}");
                throw;
            }
        }
    }
}
