using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;
using ScalesAutomation.Properties;

namespace ScalesAutomation
{
    public class MySerialReader : IDisposable
    {
        public SynchronizedCollection<Measurement> Measurements;
        public SerialPort serialPort;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        System.Windows.Forms.Timer timer;

        SynchronizedCollection<Measurement> rawMeasurements;
        Queue<byte> recievedData = new Queue<byte>();
        int requiredConsecutiveStableMeasurements;
        bool busy;
        Measurement lastAddedMeasurement;

        #region Public Methods

        public MySerialReader(SynchronizedCollection<Measurement> measurements)
        {
            Measurements = measurements;
            rawMeasurements = new SynchronizedCollection<Measurement>();
            lastAddedMeasurement.weight = -1;

            requiredConsecutiveStableMeasurements = Settings.Default.ConsecutiveStableMeasurements;

            CreateTimer();

            InitializeTransmission();
        }

        public void Dispose()
        {
            var timeout = 100;
            var finalized = false;

            while (timeout > 0)
            {
                if (!busy)
                {
                    if (serialPort != null)
                    {
                        serialPort.DataReceived -= serialPort_DataReceived;

                        if (serialPort.IsOpen)
                            serialPort.Close();

                        serialPort.Dispose();

                        finalized = true;
                    }

                    break;
                }
                else
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (!finalized)
            {
                if (serialPort != null)
                {
                    serialPort.DataReceived -= serialPort_DataReceived;

                    if (serialPort.IsOpen)
                        serialPort.Close();

                    serialPort.Dispose();
                }
            }
        }

        #endregion

        #region Events

        void timer_Tick(object sender, EventArgs e)
        {
            int stableCounter = 0; // how many times in a row same weight measurement was stable
            int previousMeasurementWeight = -1;
            Measurement currentMeasurement;
            
            /////////////////////////////////////////////////////////////////
            // take data from rawMeasurements and add it to Measurements List

            // add only stable
            // remove glitches
            // delete from rawMeasurements after adding? or keep track of last item added
            // add a global for currently added measurement so not to add 2 times

            if (rawMeasurements.Count > 0)
            {
                // keep in mid that we could already have added this measurement in the previous timer tick.
                if (lastAddedMeasurement.weight != -1)
                {
                    currentMeasurement = lastAddedMeasurement;
                    stableCounter = 5; // do not add this measurement again
                }
                else
                    currentMeasurement = rawMeasurements[0];

                for (int i = 0; i < rawMeasurements.Count; i++)
                {
                    previousMeasurementWeight = currentMeasurement.weight; // cannot use i-1 because we deleted that. Cannot save elsewhere because of multiple branches
                    currentMeasurement = rawMeasurements[i];
                    rawMeasurements.RemoveAt(i--);

                    // find first stable
                    if (!currentMeasurement.isStable)
                    {
                        // reset everything here and continue
                        stableCounter = 0;
                        lastAddedMeasurement.weight = -1;
                        continue;
                    }

                    if (previousMeasurementWeight == currentMeasurement.weight)
                    {
                        //previousMeasurement = currentMeasurement.weight;
                        stableCounter++;

                        if (stableCounter == requiredConsecutiveStableMeasurements)
                        {
                            Measurements.Add(currentMeasurement);
                            lastAddedMeasurement = currentMeasurement;
                        }
                    }
                    else
                    {
                        stableCounter = 0;
                        lastAddedMeasurement.weight = -1;
                        continue;
                    }
                }
            }
         }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            Thread myThread = new System.Threading.Thread(delegate ()
            {
                try
                {
                    if (!serialPort.IsOpen) return;

                    busy = true;

                    byte[] data = new byte[serialPort.BytesToRead];
                    log.Debug("Bytes To Read: " + serialPort.BytesToRead.ToString() + Environment.NewLine);

                    serialPort.Read(data, 0, data.Length);

                    data.ToList().ForEach(b => recievedData.Enqueue(b));

                    DiscardAllBytesUntilStart();

                    AddToRawMeasurements();

                    busy = false;
                }
                catch (Exception ex)
                {
                    log.Debug("DataReceived: " + ex.Message + Environment.NewLine);
                }
            });

            myThread.Start();

        }

        #endregion

        #region Private Methods

        void InitializeTransmission()
        {
            serialPort = new SerialPort(Settings.Default.ReadCOMPort, 4800, Parity.Even, 7, StopBits.Two);

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                EnableCyclicTransmission();
                serialPort.DataReceived += serialPort_DataReceived;
            }
        }

        void EnableCyclicTransmission()
        {
            byte[] txBuffer = PrepareCyclicTransmissionPackage();
            log.Debug("Enabling Cyclic Transmission... " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
            Thread.Sleep(10);
        }

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

            // Determine if we have a "package" in the queue
            if (recievedData.Count >= 8)
            {
                var package = Enumerable.Range(0, 8).Select(i => recievedData.Dequeue());
                var packageAsIntArray = TransformByteEnumerationToIntArray(package);

                measurement.isStable = (packageAsIntArray[1] == 0 ? true : false);
                measurement.weight = packageAsIntArray[6] + packageAsIntArray[5] * 10 + packageAsIntArray[4] * 100 + packageAsIntArray[3] * 1000 + packageAsIntArray[2] * 10000;
                measurement.timeStamp = DateTime.Now;

                log.Debug("Package Received: " + string.Join(" ", packageAsIntArray) + Environment.NewLine);
                log.Debug("Stable: " + measurement.isStable + " - Weight: " + measurement.weight + Environment.NewLine);

                rawMeasurements.Add(measurement);
            }
        }

        void DiscardAllBytesUntilStart()
        {
            var queueCount = recievedData.Count;
            for (var i = 0; i < queueCount; i++)
            {
                if (recievedData.ElementAt(0) != 0x24)
                    recievedData.Dequeue();
                else
                    break;
            }
        }

        static int[] TransformByteEnumerationToIntArray(IEnumerable<byte> package)
        {
            var packageAsByteArray = package.ToArray();
            var packageAsCharArray = Array.ConvertAll(packageAsByteArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
            var packageAsIntArray = Array.ConvertAll(packageAsCharArray, element => (int)char.GetNumericValue(element));

            return packageAsIntArray;

        }

        static Measurement Transform20gInZero(Measurement oneMeasurement)
        {
            if (oneMeasurement.weight == 20)
                oneMeasurement.weight = 0;
            return oneMeasurement;
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

        #endregion

    }
}
