﻿namespace LogParser
{
    partial class LogParser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnMakeLotInfoHeader = new System.Windows.Forms.Button();
            this.btnRenameOutput = new System.Windows.Forms.Button();
            this.btnRemoveDate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(142, 80);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(89, 46);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Parsing";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnMakeLotInfoHeader
            // 
            this.btnMakeLotInfoHeader.Location = new System.Drawing.Point(29, 185);
            this.btnMakeLotInfoHeader.Name = "btnMakeLotInfoHeader";
            this.btnMakeLotInfoHeader.Size = new System.Drawing.Size(126, 23);
            this.btnMakeLotInfoHeader.TabIndex = 1;
            this.btnMakeLotInfoHeader.Text = "Make Lot Info Header";
            this.btnMakeLotInfoHeader.UseVisualStyleBackColor = true;
            this.btnMakeLotInfoHeader.Click += new System.EventHandler(this.btnMakeLotInfoHeader_Click);
            // 
            // btnRenameOutput
            // 
            this.btnRenameOutput.Location = new System.Drawing.Point(241, 185);
            this.btnRenameOutput.Name = "btnRenameOutput";
            this.btnRenameOutput.Size = new System.Drawing.Size(91, 23);
            this.btnRenameOutput.TabIndex = 2;
            this.btnRenameOutput.Text = "Rename Output";
            this.btnRenameOutput.UseVisualStyleBackColor = true;
            this.btnRenameOutput.Click += new System.EventHandler(this.btnRenameOutput_Click);
            // 
            // btnRemoveDate
            // 
            this.btnRemoveDate.Location = new System.Drawing.Point(153, 156);
            this.btnRemoveDate.Name = "btnRemoveDate";
            this.btnRemoveDate.Size = new System.Drawing.Size(91, 23);
            this.btnRemoveDate.TabIndex = 3;
            this.btnRemoveDate.Text = "Remove Date";
            this.btnRemoveDate.UseVisualStyleBackColor = true;
            this.btnRemoveDate.Click += new System.EventHandler(this.btnRemoveDate_Click);
            // 
            // LogParser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 226);
            this.Controls.Add(this.btnRemoveDate);
            this.Controls.Add(this.btnRenameOutput);
            this.Controls.Add(this.btnMakeLotInfoHeader);
            this.Controls.Add(this.btnStart);
            this.Name = "LogParser";
            this.Text = "Log Parser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnMakeLotInfoHeader;
        private System.Windows.Forms.Button btnRenameOutput;
        private System.Windows.Forms.Button btnRemoveDate;
    }
}

