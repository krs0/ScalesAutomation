using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;
using ScalesAutomation.Properties;
using System.Collections.Concurrent;
using System.Collections;

namespace ScalesAutomation
{
    public class MySerialReader : IDisposable
    {
        public SynchronizedCollection<Measurement> Measurements;
        public SerialPort serialPort;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        System.Windows.Forms.Timer timer;

        SynchronizedCollection<Measurement> rawMeasurements;
        byte SerialPackageLength = 0; // package length (row length) transmitted by the scales
        double zeroThreshold;
        double userTare; // Tare entered by the user in GUI. We need it here to check with the Tare received from scale.
        ConcurrentQueue<byte> recievedData = new ConcurrentQueue<byte>();
        int requiredConsecutiveStableMeasurements;
        bool busy;
        Measurement lastAddedMeasurement;
        int stableCounter = 0; // how many times in a row same weight measurement was stable

        #region Public Methods

        public MySerialReader(SynchronizedCollection<Measurement> measurements, double zeroThreshold, double userTare)
        {
            Measurements = measurements;
            rawMeasurements = new SynchronizedCollection<Measurement>();
            this.zeroThreshold = zeroThreshold;
            this.userTare= userTare;
            lastAddedMeasurement.TotalWeight = -1;

            requiredConsecutiveStableMeasurements = Settings.Default.ConsecutiveStableMeasurements;

            switch(Settings.Default.ScaleType)
            {
                case "Bilanciai":
                    SerialPackageLength = 8;
                    break;
                case "Constalaris":
                    SerialPackageLength = 17;
                    break;
            }

            CreateTimer();

            InitializeTransmission();
        }

        public void Dispose()
        {
            var timeout = 1000;
            var finalized = false;

            while(timeout > 0)
            {
                if(!busy)
                {
                    SerialPortClose();
                    finalized = true;

                    break;
                }
                else
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if(!finalized)
                SerialPortClose();
        }

        #endregion

        #region Events

        void timer_Tick(object sender, EventArgs e)
        {
            int previousMeasurementTotalWeight = -1;
            Measurement currentMeasurement;
            
            /////////////////////////////////////////////////////////////////
            // take data from rawMeasurements and add it to Measurements List

            // add only stable
            // remove glitches
            // delete from rawMeasurements after adding? or keep track of last item added
            // add a global for currently added measurement so not to add 2 times

            if (rawMeasurements.Count > 0)
            {
                // we could already have added this measurement in the previous timer tick but it still continues as a stable measurement
                if (lastAddedMeasurement.TotalWeight != -1)
                {
                    currentMeasurement = lastAddedMeasurement;
                }
                else
                    currentMeasurement = rawMeasurements[0];

                for (int i = 0; i < rawMeasurements.Count; i++)
                {
                    previousMeasurementTotalWeight = currentMeasurement.TotalWeight; // cannot use i-1 because we deleted that. Cannot save elsewhere because of multiple branches
                    currentMeasurement = rawMeasurements[i];
                    rawMeasurements.RemoveAt(i--);

                    // find first stable
                    if (!currentMeasurement.IsStable)
                    {
                        // reset everything here and continue
                        stableCounter = 0;
                        lastAddedMeasurement.TotalWeight = -1;
                        continue;
                    }

                    if (previousMeasurementTotalWeight == currentMeasurement.TotalWeight)
                    {
                        stableCounter++;

                        if (stableCounter == requiredConsecutiveStableMeasurements - 1) // Add measurement only once when stableCounter is just right, ignore consecutive stable measurements. (Starts at 0).
                        {
                            Measurements.Add(currentMeasurement);
                            lastAddedMeasurement = currentMeasurement;
                        }
                    }
                    else
                    {
                        stableCounter = 0;
                        lastAddedMeasurement.TotalWeight = -1;
                        continue;
                    }
                }
            }
         }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            Thread myThread = new System.Threading.Thread(delegate ()
            {
                lock(this)
                {
                    try
                    {
                        if(!serialPort.IsOpen) return;

                        busy = true;

                        //log.Debug("Bytes To Read: " + serialPort.BytesToRead.ToString() + Environment.NewLine);

                        if(serialPort.BytesToRead < 2 * SerialPackageLength) return;

                        if(Settings.Default.ScaleType == "Bilanciai")
                        {
                            byte[] data = new byte[serialPort.BytesToRead];
                            var bytesRed = serialPort.Read(data, 0, data.Length);
                            log.Debug("Bytes Read: " + bytesRed.ToString() + Environment.NewLine);

                            data.ToList().ForEach(b => recievedData.Enqueue(b));

                            DiscardAllBytesUntilStart();

                            AddToRawMeasurements();
                        }
                        if(Settings.Default.ScaleType == "Constalaris")
                        {
                            var readData = serialPort.ReadLine();
                            log.Debug("Recieved Data: " + readData + Environment.NewLine);

                            if(readData == null || readData.Count() != SerialPackageLength - 1) return; // discard wrong packages

                            readData.ToList().ForEach(b => recievedData.Enqueue(Convert.ToByte(b)));
                            log.Debug("Recieved Data size: " + recievedData.Count.ToString() + Environment.NewLine);

                            AddToRawMeasurements();

                            while(recievedData.TryDequeue(out byte item)) { }
                        }

                        busy = false;
                    }
                    catch(Exception ex)
                    {
                        log.Error("DataReceived: " + ex.Message + Environment.NewLine);
                    }
                }
            });

            myThread.Start();

        }

        #endregion

        #region Private Methods

        void InitializeTransmission()
        {
            var scaleType = Settings.Default.ScaleType;

            if (scaleType == "Bilanciai")
                serialPort = new SerialPort(Settings.Default.ReadCOMPort, 4800, Parity.Even, 7, StopBits.Two);
            else if (scaleType == "Constalaris")
                serialPort = new SerialPort(Settings.Default.ReadCOMPort, 9600, Parity.None, 8, StopBits.One);
            else
                log.Error("Model cantar incorect: " + scaleType + " - Modelele suportate sunt: Bilanciai sau Constalaris");

            serialPort.ReadTimeout = 1000;
            serialPort.ReceivedBytesThreshold = SerialPackageLength;

            if(!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();

                    if(scaleType == "Bilanciai")
                        EnableCyclicTransmission(); // we need to write a command for Bilanciai to start sending data

                    serialPort.DataReceived += serialPort_DataReceived;
                }
                catch(Exception)
                {
                    log.Error("Nu se poate initializa legatura cu cantarul pe portul: " + serialPort.PortName + ". Verificati conexiunea cu cantarul, sau valoarea portului COM");
                }
            }
        }

        /// <summary>
        /// Sends over serial an Enable Cyclic Transmission package. Valid only for Bilanciai Scales.
        /// </summary>
        void EnableCyclicTransmission()
        {
            byte[] txBuffer = PrepareCyclicTransmissionPackage();
            log.Info("Enabling Cyclic Transmission... " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            var s = serialPort.IsOpen;

            serialPort.Write(txBuffer, 0, txBuffer.Length);
            Thread.Sleep(10);
        }

        /// <summary>
        /// Preapare Initialize Cycling transmission Package for Bilanciai Scales
        /// </summary>
        /// <returns>Initialize Cycling Transmission Package</returns>
        byte[] PrepareCyclicTransmissionPackage()
        {
            byte[] txBuffer = new byte[3];

            // assign to buffer
            txBuffer[0] = 0x73;
            txBuffer[1] = 0x78;
            txBuffer[2] = 0x0D; // CR = end character

            return txBuffer;

        }

        void AddToRawMeasurements()
        {
            var measurement = new Measurement();
            byte[] packageAsByteArray = new byte[] { };

            try
            {
                // Determine if we have a full "package" in the queue
                // ToDo: CrLa - Here we could lose data. change to while and test
                if(recievedData.Count >= SerialPackageLength - 1)
                {
                    //var package = Enumerable.Range(0, SerialPackageLength-1).Select(i => recievedData.Dequeue()); // Old version of code
                    var package = new List<byte>();
                    for(int i = 0; i < SerialPackageLength - 2; i++)
                    {
                        recievedData.TryDequeue(out byte val);
                        package.Add(val);
                    }
                    var packageAsIntArray = TransformByteEnumerationToIntArray(package, ref packageAsByteArray);

                    switch(Settings.Default.ScaleType)
                    {
                        case "Bilanciai":
                            measurement.IsStable = (packageAsIntArray[1] == 0 ? true : false);
                            measurement.TotalWeight = packageAsIntArray[6] + packageAsIntArray[5] * 10 + packageAsIntArray[4] * 100 + packageAsIntArray[3] * 1000 + packageAsIntArray[2] * 10000;
                            break;
                        case "Constalaris":
                            measurement.IsStable = (package.ElementAt(1) == 84 ? true : false); // if second element is T (from ST) than its stable
                            measurement.TotalWeight = packageAsIntArray[12] + packageAsIntArray[11] * 10 + packageAsIntArray[10] * 100 + packageAsIntArray[8] * 1000;
                            if(packageAsIntArray[7] != -1) // if tens for Kg is a valid number (if not used is space that was previously converted to -1)
                                measurement.TotalWeight += packageAsIntArray[7] * 10000;
                            if(package.ElementAt(7) == 45) // if negative measurement sent (this is actually Tare) set to 0
                            {
                                // sanity check for Tare that is the same as in GUI... but only silent error in log!
                                var tareOnScale = measurement.TotalWeight;
                                if(tareOnScale != this.userTare)
                                    log.Error(String.Format("Error: Tara raportata de cantar: {0} nu este aceiasi cu tara setata in GUI: {1}!", tareOnScale, this.userTare));

                                measurement.TotalWeight = 0; // report as 0 measurement (to match Bialnciai)
                            }
                            break;
                    }

                    measurement.TimeStamp = DateTime.Now;

                    // Logging long format
                    // log.Info("Package Received: " + BitConverter.ToString(packageAsByteArray) + "  Stable: " + (measurement.IsStable ? "T" : "F") + " - Weight: " + measurement.TotalWeight);

                    // Logging short format
                    log.Info("S: " + (measurement.IsStable ? "T" : "F") + " - W: " + measurement.TotalWeight);

                    // Everything up to ZeroThreshold grams is converted to 0
                    ApplyZeroThresholdCorrection(ref measurement);

                    rawMeasurements.Add(measurement);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        void DiscardAllBytesUntilStart()
        {
            var queueCount = recievedData.Count;

            try
            {
                for(var i = 0; i < queueCount; i++)
                {
                    if(recievedData.ElementAt(0) != 0x24)
                        recievedData.TryDequeue(out byte readValue);
                    else
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static int[] TransformByteEnumerationToIntArray(IEnumerable<byte> package, ref byte[] packageAsByteArray)
        {
            packageAsByteArray = package.ToArray();
            var packageAsCharArray = Array.ConvertAll(packageAsByteArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
            var packageAsIntArray = Array.ConvertAll(packageAsCharArray, element => (int)char.GetNumericValue(element));

            return packageAsIntArray;

        }

        /// <summary>
        /// Make corrections here for measurements close to Zero but not really 0 and sometimes even not Stable
        /// The measured weight is compared with the zero threshhold defined in settings and if lower, 0 is put instead and the measurement is forced to stable!
        /// </summary>
        bool ApplyZeroThresholdCorrection(ref Measurement measurement)
        {
            bool result = true;

            if ((measurement.TotalWeight < zeroThreshold) && !(measurement.TotalWeight == 0 && measurement.IsStable))
            {
                measurement.TotalWeight = 0;
                measurement.IsStable = true;
                log.Debug("S: " + (measurement.IsStable ? "T" : "F") + " - W: " + measurement.TotalWeight);
                result = false;
            }

            return result;
        }

        void CreateTimer()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 300,
                Enabled = true
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void SerialPortClose()
        {
            try
            {
                if(serialPort != null)
                {
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;
                    serialPort.DataReceived -= serialPort_DataReceived;

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
                log.Error("Cannot Dispose of serial port:" + ex.Message + Environment.NewLine);
                throw;
            }
        }

        #endregion

    }
}
