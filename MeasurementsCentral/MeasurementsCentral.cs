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

        public MeasurementsCentral()
        {
            InitializeComponent();

            lvwMeasurementsFiles.ListViewItemSorter = lvwColumnSorter;
            lvwColumnSorter = new ListViewColumnSorter();

            dlgFolderBrowser = new FolderBrowserDialog();
            dlgFolderBrowser.RootFolder = Environment.SpecialFolder.Recent;

            // Create a new ToolTip
            tooltip = new ToolTip();
            tooltip.IsBalloon = true;

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
            tbFileName.Text = path;

            // Get all files in the directory
            string[] files = Directory.GetFiles(path);

            lvwMeasurementsFiles.Items.Clear();

            // Add each file to the ListView
            foreach(string file in files)
            {
                var filename = Path.GetFileName(file).Trim();
                ListViewItem item = new ListViewItem(filename);
                item.SubItems.Add("OK");
                lvwMeasurementsFiles.Items.Add(item);

                // Add the file name to the tooltip
                tooltip.SetToolTip(lvwMeasurementsFiles, filename);
            }
        }

        private void lvwMeasurementsFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if(e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if(lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
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