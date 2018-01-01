using System;
using System.Windows.Forms;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace ScalesAutomation
{
    public partial class ScalesAutomation : Form
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MySerialReader readPort;
        private MySerialWriter writePort;
        private Thread writeThread;
        private Thread readThread;
        private String code;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        
        public struct measurement
        {
            public bool isStable;
            public int weight;
        }

        public SynchronizedCollection<measurement> Measurements = new SynchronizedCollection<measurement>();

        public DataTable dt = new DataTable();

        private BindingList<measurement> bindingList;
        private BindingSource source;

        public ScalesAutomation()
        {
            InitializeComponent();

            dt.Columns.Add("#", typeof(int));
            dt.Columns.Add("Weight", typeof(string));

            //bindingList = new BindingList<measurement>(Measurements);
            //source = new BindingSource(bindingList, null);
            // dataGridViewMain.DataSource = bindingList;
            dataGridViewMain.DataSource = dt;
            //dataGridViewMain.DataBind();


            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (2);              // Timer will tick evert 5 second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (Measurements.Count > 0)
            {
                var nrOfRowsInDataTable = dt.Rows.Count;
                for (int i = nrOfRowsInDataTable, j = 0; i < nrOfRowsInDataTable + Measurements.Count; i++, j++)
                {
                    var row = dt.NewRow();
                    row["#"] = i;
                    row["Weight"] = Measurements[j].weight;
                    dt.Rows.Add(row);

                    // Add to excel


                    log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"]);
                }
            }

            CreateCSVFile(ref dt, "D:\\a.csv");

            // bindingList = new BindingList<measurement>(Measurements);
            //dataGridViewMain.DataSource = bindingList;
            dataGridViewMain.Refresh();
            Measurements.Clear();
            //dataGridViewMain.DataSource = dt;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button OK Clicked" + Environment.NewLine);

            if (readPort != null)
                readPort.Dispose();

            if (writePort != null)
                writePort.Dispose();

            if (writeThread != null)
                writeThread.Abort();

            if (readThread != null)
                readThread.Abort();

            writeThread = new Thread(new ThreadStart(WriteThread));
            readThread = new Thread(new ThreadStart(ReadThread));

            readThread.Start();

            Thread.Sleep(100);
            writeThread.Start();
        }

        private void ReadThread()
        {
            readPort = new MySerialReader(Measurements);
        }

        private void WriteThread()
        {
            writePort = new MySerialWriter();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button Stop Clicked" + Environment.NewLine);

            readPort.Dispose();
            writePort.Dispose();
        }

        private void txtCode_Validated(object sender, EventArgs e)
        {
            code = txtCode.Text;
        }

        public void CreateCSVFile(ref DataTable dt, string strFilePath)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                int iColCount = dt.Columns.Count;
                for (int i = 0; i < iColCount; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}