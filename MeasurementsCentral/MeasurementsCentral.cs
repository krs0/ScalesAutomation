using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace MeasurementsCentral
{
    public partial class MeasurementsCentral : Form
    {
        public MeasurementsCentral()
        {
            InitializeComponent();

            lvwColumnSorter = new ListViewColumnSorter();
            this.listView.ListViewItemSorter = lvwColumnSorter;

            dialog = new FolderBrowserDialog();
            //Set Root folder as desktop
            dialog.RootFolder = Environment.SpecialFolder.Recent;

            // Create a new ToolTip
            tooltip = new ToolTip();
            tooltip.IsBalloon = true;
        }

        FolderBrowserDialog dialog;
        ListView listView;
        ListViewColumnSorter lvwColumnSorter;
        ToolTip tooltip;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                tbFileName.Text = path;

                // Get all files in the directory
                string[] files = Directory.GetFiles(path);

                listView.Items.Clear();

                // Add each file to the ListView
                foreach(string file in files)
                {
                    var filename = Path.GetFileName(file).Trim();
                    ListViewItem item = new ListViewItem(filename);
                    item.SubItems.Add("OK");
                    listView.Items.Add(item);

                    // Add the file name to the tooltip
                    tooltip.SetToolTip(listView, filename);
                }
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
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
            this.listView.Sort();
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