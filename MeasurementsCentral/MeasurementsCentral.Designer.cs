﻿namespace MeasurementsCentral
{
    partial class MeasurementsCentral
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.FileName = new System.Windows.Forms.ColumnHeader();
            this.Status = new System.Windows.Forms.ColumnHeader();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileName,
            this.Status});
            this.listView.Location = new System.Drawing.Point(12, 54);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(982, 593);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
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
            this.tbFileName.Location = new System.Drawing.Point(12, 16);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(895, 27);
            this.tbFileName.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(919, 12);
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
    }
}