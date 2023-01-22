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
            dialog.RootFolder = Environment.SpecialFolder.Desktop;

            // Add a column for the file size
            listView.Columns.Add("Size", 100, HorizontalAlignment.Right);

            // Create a new ToolTip
            tooltip = new ToolTip();
            tooltip.IsBalloon = true;
        }

        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Location = new System.Drawing.Point(12, 54);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(982, 606);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(99, 17);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(895, 27);
            this.tbFileName.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 15);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 31);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // MeasurementsCentral
            // 
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.listView);
            this.Name = "MeasurementsCentral";
            this.Text = "Centralizator Masuratori";
            this.ResumeLayout(false);
            this.PerformLayout();

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