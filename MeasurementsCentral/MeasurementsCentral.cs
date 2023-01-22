using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
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
            lvMeasurementFiles.Columns.Add("Size", 100, HorizontalAlignment.Right);

            // Create a new ToolTip
            tooltip = new ToolTip();
            tooltip.IsBalloon = true;
        }

        private void InitializeComponent()
        {
            this.lvMeasurementFiles = new System.Windows.Forms.ListView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvMeasurementFiles
            // 
            this.lvMeasurementFiles.Location = new System.Drawing.Point(12, 54);
            this.lvMeasurementFiles.Name = "lvMeasurementFiles";
            this.lvMeasurementFiles.Size = new System.Drawing.Size(982, 606);
            this.lvMeasurementFiles.TabIndex = 0;
            this.lvMeasurementFiles.UseCompatibleStateImageBehavior = false;
            this.lvMeasurementFiles.View = System.Windows.Forms.View.Details;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(99, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(895, 27);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 0;
            this.button1.Text = "Browse";
            // 
            // MeasurementsCentral
            // 
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lvMeasurementFiles);
            this.Name = "MeasurementsCentral";
            this.Text = "Centralizator Masuratori";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected folder path
                string path = dialog.SelectedPath;

                // Get all files in the directory
                string[] files = Directory.GetFiles(path);

                // Clear any existing items in the ListView
                lvMeasurementFiles.Items.Clear();

                // Add each file to the ListView
                foreach(string file in files)
                {
                    ListViewItem item = new ListViewItem(file);
                    lvMeasurementFiles.Items.Add(item);
                }
            }
        }

        FolderBrowserDialog dialog;
        ListView lvMeasurementFiles;
        ToolTip tooltip;

    }
}