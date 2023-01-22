using System;
using System.IO;
using System.Windows.Forms;

namespace MeasurementsCentral
{
    public partial class MeasurementsCentral : Form
    {
        public MeasurementsCentral()
        {
            InitializeComponent();

            dialog = new FolderBrowserDialog();
            //Set Root folder as desktop
            dialog.RootFolder = Environment.SpecialFolder.Recent;

            // Create a new ToolTip
            tooltip = new ToolTip();
            tooltip.IsBalloon = true;
        }

        FolderBrowserDialog dialog;
        ListView listView;
        ToolTip tooltip;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected folder path
                string path = dialog.SelectedPath;
                // Show the selected folder path in the TextBox
                tbFileName.Text = path;

                // Get all files in the directory
                string[] files = Directory.GetFiles(path);

                // Clear any existing items in the ListView
                listView.Items.Clear();

                // Add each file to the ListView
                foreach(string file in files)
                {
                    ListViewItem item = new ListViewItem(file);
                    FileInfo fileInfo = new FileInfo(file);
                    item.SubItems.Add(fileInfo.Length.ToString());
                    listView.Items.Add(item);

                    // Add the file name to the tooltip
                    tooltip.SetToolTip(listView, file);
                }
            }
        }
    }
}