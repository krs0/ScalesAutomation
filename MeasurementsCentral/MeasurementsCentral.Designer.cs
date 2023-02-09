using System.Windows.Forms;

namespace MeasurementsCentral
{
    partial class MeasurementsCentral
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.lvwMeasurementsFiles = new System.Windows.Forms.ListView();
            this.FileName = new System.Windows.Forms.ColumnHeader();
            this.Status = new System.Windows.Forms.ColumnHeader();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblMeasurementsFolder = new System.Windows.Forms.Label();
            this.lblMaxNoOfProcessedFiles = new System.Windows.Forms.Label();
            this.numUDMaxFileNumber = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUDMaxFileNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwMeasurementsFiles
            // 
            this.lvwMeasurementsFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileName,
            this.Status});
            this.lvwMeasurementsFiles.FullRowSelect = true;
            this.lvwMeasurementsFiles.Location = new System.Drawing.Point(12, 97);
            this.lvwMeasurementsFiles.Name = "lvwMeasurementsFiles";
            this.lvwMeasurementsFiles.Size = new System.Drawing.Size(982, 593);
            this.lvwMeasurementsFiles.TabIndex = 4;
            this.lvwMeasurementsFiles.UseCompatibleStateImageBehavior = false;
            this.lvwMeasurementsFiles.View = System.Windows.Forms.View.Details;
            this.lvwMeasurementsFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwMeasurementsFiles_ColumnClick);
            this.lvwMeasurementsFiles.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lvwMeasurementsFiles_KeyPress);
            this.lvwMeasurementsFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwMeasurementsFiles_MouseDoubleClick);
            // 
            // FileName
            // 
            this.FileName.Text = "Nume Fisier";
            this.FileName.Width = 900;
            // 
            // Status
            // 
            this.Status.Text = "Status";
            this.Status.Width = 90;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(159, 16);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(712, 27);
            this.tbFileName.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(895, 14);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(99, 31);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblMeasurementsFolder
            // 
            this.lblMeasurementsFolder.AutoSize = true;
            this.lblMeasurementsFolder.Location = new System.Drawing.Point(12, 19);
            this.lblMeasurementsFolder.Name = "lblMeasurementsFolder";
            this.lblMeasurementsFolder.Size = new System.Drawing.Size(141, 20);
            this.lblMeasurementsFolder.TabIndex = 2;
            this.lblMeasurementsFolder.Text = "Director Masuratori:";
            // 
            // lblMaxNoOfProcessedFiles
            // 
            this.lblMaxNoOfProcessedFiles.AutoSize = true;
            this.lblMaxNoOfProcessedFiles.Location = new System.Drawing.Point(12, 59);
            this.lblMaxNoOfProcessedFiles.Name = "lblMaxNoOfProcessedFiles";
            this.lblMaxNoOfProcessedFiles.Size = new System.Drawing.Size(227, 20);
            this.lblMaxNoOfProcessedFiles.TabIndex = 3;
            this.lblMaxNoOfProcessedFiles.Text = "Numarul maxim de fisiere listate:";
            // 
            // numUDMaxFileNumber
            // 
            this.numUDMaxFileNumber.Location = new System.Drawing.Point(247, 57);
            this.numUDMaxFileNumber.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numUDMaxFileNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUDMaxFileNumber.Name = "numUDMaxFileNumber";
            this.numUDMaxFileNumber.Size = new System.Drawing.Size(59, 27);
            this.numUDMaxFileNumber.TabIndex = 3;
            this.numUDMaxFileNumber.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numUDMaxFileNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numUDMaxFileNumber_KeyPress);
            this.numUDMaxFileNumber.Validated += new System.EventHandler(this.numUDMaxFileNumber_Validated);
            // 
            // MeasurementsCentral
            // 
            this.ClientSize = new System.Drawing.Size(1006, 703);
            this.Controls.Add(this.numUDMaxFileNumber);
            this.Controls.Add(this.lblMaxNoOfProcessedFiles);
            this.Controls.Add(this.lblMeasurementsFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.lvwMeasurementsFiles);
            this.Name = "MeasurementsCentral";
            this.Text = "Centralizator Masuratori";
            ((System.ComponentModel.ISupportInitialize)(this.numUDMaxFileNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.ClientSize = new System.Drawing.Size(800, 450);
        //    this.Text = "Centralizator Masuratori";
        //}

        #endregion

        private TextBox tbFileName;
        private Button btnBrowse;
        private ColumnHeader FileName;
        private ColumnHeader Status;
        private Label lblMeasurementsFolder;
        private Label lblMaxNoOfProcessedFiles;
        private NumericUpDown numUDMaxFileNumber;
    }
}