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
        bool alreadyAddedToList;
        bool isFirstMeasurement = true;
        int lastMeasurement;
        bool busy;

        #region Public Methods

        public MySerialReader(SynchronizedCollection<Measurement> measurements)
        {
            Measurements = measurements;
            rawMeasurements = new SynchronizedCollection<Measurement>();

            InitializeTransmission();

        }

        public void EnableCyclicTransmission()
        {
            byte[] txBuffer = PrepareCyclicTransmissionPackage();
            log.Debug("Enabling Cyclic Transmission... " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
            Thread.Sleep(10);
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
            take data from rawMeasurements and add it to Measurements List
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

        private byte[] PrepareCyclicTransmissionPackage()
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

                // addMeasurement only if stable, only if not already added to list and
                // only if not first measurement (scales bug)
        //        if (measurement.isStable)
        //        {
        //            if (!alreadyAddedToList)
        //            {
        //                if (!isFirstMeasurement)
        //                {
        //                    measurement = Transform20gInZero(measurement);
        //                    Measurements.Add(measurement);
        //                    lastMeasurement = measurement.weight;
        //                    alreadyAddedToList = true;
        //                }
        //                else
        //                {
        //                    isFirstMeasurement = false;
        //                }
        //            }
        //            else
        //            {
        //                if (lastMeasurement != measurement.weight)
        //                {
        //                    log.Error("Measurement added to list does not match current measurement!" + Environment.NewLine);
        //                    // Scales bug transmits as stable last unstable measurement
        //                }
        //            }
        //        }
        //        else
        //        {
        //            isFirstMeasurement = true;
        //            alreadyAddedToList = false;
        //        }

        //        recievedData.Clear();
        //        log.Debug("Measurements in list: " + Measurements.Count + Environment.NewLine);
        //    }
        //}

        private void DiscardAllBytesUntilStart()
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

        private static int[] TransformByteEnumerationToIntArray(IEnumerable<byte> package)
        {
            var packageAsByteArray = package.ToArray();
            var packageAsCharArray = Array.ConvertAll(packageAsByteArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
            var packageAsIntArray = Array.ConvertAll(packageAsCharArray, element => (int)char.GetNumericValue(element));

            return packageAsIntArray;

        }

        private static Measurement Transform20gInZero(Measurement oneMeasurement)
        {
            if (oneMeasurement.weight == 20)
                oneMeasurement.weight = 0;
            return oneMeasurement;
        }

        private void CreateTimer()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 500,
                Enabled = true
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        #endregion

    }
}
