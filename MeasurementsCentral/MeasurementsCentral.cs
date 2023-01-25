using ScalesAutomation;
using MeasurementsCentral.Properties;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace MeasurementsCentral
{
    public partial class MeasurementsCentral : Form
    {
        FolderBrowserDialog dlgFolderBrowser;
        ListView lvwMeasurementsFiles;
        ListViewColumnSorter lvwColumnSorter;
        ToolTip tooltip;
        string CSVServerFolderPath = ScalesAutomation.Common.TransformToAbsolutePath(Common.Properties.Settings.Default.CSVServerFolderPath);

        public MeasurementsCentral()
        {
            ImageList imageList = new ImageList();

            imageList.ImageSize = new Size(32, 32);

            // Add images to the image list
            imageList.Images.Add(Image.FromFile("Images/ok.png"));
            imageList.Images.Add(Image.FromFile("Images/x.png"));

            InitializeComponent();

            Shown += MeasurementsCentral_Shown;

            lvwColumnSorter = new ListViewColumnSorter();
            lvwMeasurementsFiles.ListViewItemSorter = lvwColumnSorter;

            lvwMeasurementsFiles.ShowItemToolTips = true;

            dlgFolderBrowser = new FolderBrowserDialog();
            dlgFolderBrowser.RootFolder = Environment.SpecialFolder.Recent;

            lvwMeasurementsFiles.SmallImageList = imageList;
        }

        private void MeasurementsCentral_Shown(object sender, EventArgs e)
        {
            PpopulatelvwMeasurementsFiles(Settings.Default.LastSelectedFolder);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                string path = dlgFolderBrowser.SelectedPath;

                PpopulatelvwMeasurementsFiles(path);
            }
        }

        private void PpopulatelvwMeasurementsFiles(string path)
        {
            int fileCount = 0;

            tbFileName.Text = path;

            // Get all filePaths in the directory
            string[] filePaths = Directory.GetFiles(path);

            lvwMeasurementsFiles.Items.Clear();

            // Add each measurements file to the ListView
            foreach(string measuremetnsFilePath in filePaths)
            {
                var measurementsFilename = Path.GetFileName(measuremetnsFilePath).Trim();
                ListViewItem item = new ListViewItem(measurementsFilename);
                item.Tag = measuremetnsFilePath;
                //item.SubItems.Add("OK");
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));

                var metrologyResult = StartMetrologyReader.GetMetrologyResults(measurementsFilename, CSVServerFolderPath);

                if(metrologyResult == "Lot Acceptat")
                    item.ImageIndex = 0;
                else
                    item.ImageIndex = 1;

                // Add the measuremetnsFilePath name to the tooltip
                item.ToolTipText = measurementsFilename;
                lvwMeasurementsFiles.Items.Add(item);

                if (fileCount > Settings.Default.MaxNoOfMeasurementFiles)
                    break;

                fileCount++;
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
    }

    class ListViewColumnSorter : IComparer
    {
        private int ColumnToSort;
        private SortOrder OrderOfSort;

        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;
            string xText = listviewX.SubItems[ColumnToSort].Text;
            string yText = listviewY.SubItems[ColumnToSort].Text;
            compareResult = String.Compare(xText, yText);
            if(OrderOfSort == SortOrder.Ascending)
            {
                return compareResult;
            }
            else if(OrderOfSort == SortOrder.Descending)
            {
                return (-compareResult);
            }
            else
            {
                return 0;
            }
        }
    }
}