namespace ScalesAutomation
{
    partial class ScalesAutomation
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
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.dataGridViewMain = new System.Windows.Forms.DataGridView();
            this.measurementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.measurementBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(55, 63);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(160, 63);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(55, 26);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(180, 20);
            this.txtCode.TabIndex = 2;
            this.txtCode.Validated += new System.EventHandler(this.txtCode_Validated);
            // 
            // dataGridViewMain
            // 
            this.dataGridViewMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMain.Location = new System.Drawing.Point(257, 26);
            this.dataGridViewMain.Name = "dataGridViewMain";
            this.dataGridViewMain.Size = new System.Drawing.Size(180, 435);
            this.dataGridViewMain.TabIndex = 3;
            // 
            // ScalesAutomation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 540);
            this.Controls.Add(this.dataGridViewMain);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "ScalesAutomation";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.DataGridView dataGridViewMain;
        private System.Windows.Forms.BindingSource measurementBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource1;
        private System.Windows.Forms.BindingSource measurementBindingSource1;
    }
}

