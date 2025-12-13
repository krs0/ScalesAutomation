using MetrologyReaderNS;
using MeasurementsCentral.Properties;
using System.Collections;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using log4net;
using System.Reflection;
using CommonNS;

namespace MeasurementsCentral
{
    public partial class MeasurementsCentral : Form
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");

        FolderBrowserDialog dlgFolderBrowser;
        ListView lvwMeasurementsFiles;
        ListViewColumnSorter lvwColumnSorter;

        public MeasurementsCentral()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.Images.Add(Image.FromFile("Images/ok.png"));
            imageList.Images.Add(Image.FromFile("Images/x.png"));

            InitializeComponent();

            Shown += MeasurementsCentral_Shown; // Start filling the measurements file list only after main Form is Shown.

            numUDMaxFileNumber.Value = Settings.Default.MaxNoOfMeasurementFiles;

            lvwColumnSorter = new ListViewColumnSorter();
            lvwMeasurementsFiles.SmallImageList = imageList;
            lvwMeasurementsFiles.ListViewItemSorter = lvwColumnSorter;
            lvwMeasurementsFiles.ShowItemToolTips = true;

            dlgFolderBrowser = new FolderBrowserDialog();
            dlgFolderBrowser.RootFolder = Environment.SpecialFolder.Recent;
            tbFileName.Text = Settings.Default.LastSelectedFolder;
        }

        private void PpopulatelvwMeasurementsFiles()
        {
            if(OpenExcel.CloseExcelIfOpen())
                return;

            int fileCount = 1;

            string measurementsPath = tbFileName.Text;

            // Get all filePaths in the directory sorted newest first
            string[] filePaths = Directory.GetFiles(measurementsPath);
            Array.Sort(filePaths);
            Array.Reverse(filePaths);

            lvwMeasurementsFiles.Items.Clear();

            var metrologyReader = new MetrologyReader();
            metrologyReader.InitializeExcel($"{measurementsPath}\\..\\CentralizatorMasuratori.xlsm");

            // Add each measurements file to the ListView
            foreach(string measuremetnsFilePath in filePaths)
            {
                var measurementsFilename = Path.GetFileName(measuremetnsFilePath).Trim();
                ListViewItem item = new ListViewItem(measurementsFilename);
                item.Tag = measuremetnsFilePath;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));

                var metrologyResult = metrologyReader.GetMetrologyResult(measurementsFilename);

                if(metrologyResult == "Lot Acceptat")
                    item.ImageIndex = 0;
                else
                    item.ImageIndex = 1;

                // Add the measuremetnsFilePath name to the tooltip
                item.ToolTipText = measurementsFilename;
                lvwMeasurementsFiles.Items.Add(item);

                if(lvwMeasurementsFiles.Items.Count > 0)
                    lvwMeasurementsFiles.EnsureVisible(lvwMeasurementsFiles.Items.Count - 1);

                if (fileCount >= Settings.Default.MaxNoOfMeasurementFiles)
                    break;

                fileCount++;
            }

            metrologyReader.Dispose();
        }

        private void OpenCentralizatorMasuratori(string measurementsFilename)
        {
            if(OpenExcel.CloseExcelIfOpen())
                return;

            string measurementsPath = tbFileName.Text;

            var metrologyReader = new MetrologyReader();
            metrologyReader.InitializeExcel($"{measurementsPath}\\..\\CentralizatorMasuratori.xlsm");

            var metrologyResult = metrologyReader.GetMetrologyResult(measurementsFilename);

            // Make the workbook visible and detach it from MetrologyReader so we don't start Excel a second time.
            metrologyReader.DetachAndMakeVisible();

            // Dispose the reader but do not close the Excel instance (DetachAndMakeVisible prevents CloseExcel()).
            metrologyReader.Dispose();
        }

        private void MeasurementsCentral_Shown(object sender, EventArgs e)
        {
            PpopulatelvwMeasurementsFiles();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                string measurementsFolderPath = dlgFolderBrowser.SelectedPath;
                tbFileName.Text = measurementsFolderPath;
                Settings.Default["LastSelectedFolder"] = measurementsFolderPath;
                Settings.Default.Save();

                PpopulatelvwMeasurementsFiles();
            }
        }

        private void lvwMeasurementsFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if(e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if(lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvwMeasurementsFiles.Sort();
        }

        private void lvwMeasurementsFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var measurementsFilename = lvwMeasurementsFiles.SelectedItems[0].Text;
            OpenCentralizatorMasuratori(measurementsFilename);
        }

        private void lvwMeasurementsFiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                if(lvwMeasurementsFiles.SelectedItems.Count > 0)
                {
                    var measurementsFilename = lvwMeasurementsFiles.SelectedItems[0].Text;
                    OpenCentralizatorMasuratori(measurementsFilename);
                }
            }
        }

        private void numUDMaxFileNumber_Validated(object sender, EventArgs e)
        {
            Settings.Default["MaxNoOfMeasurementFiles"] = (int)numUDMaxFileNumber.Value;
            Settings.Default.Save();

            PpopulatelvwMeasurementsFiles();
        }

        private void numUDMaxFileNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                // Set focus to next control in tab order
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }
    }

    class ListViewColumnSorter : IComparer
    {
        public int SortColumn { set; get; }

        public SortOrder Order { set; get; }

        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;
            string xText = listviewX.SubItems[SortColumn].Text;
            string yText = listviewY.SubItems[SortColumn].Text;
            compareResult = String.Compare(xText, yText);

            if(Order == SortOrder.Ascending)
                return compareResult;
            else if(Order == SortOrder.Descending)
                return (-compareResult);
            else
                return 0;
        }
    }
}