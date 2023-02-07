namespace ScalesAutomation
{
    partial class ScalesAutomationForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScalesAutomationForm));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.dataGridViewMeasurements = new System.Windows.Forms.DataGridView();
            this.measurementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.measurementBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.btnStopLot = new System.Windows.Forms.Button();
            this.uctlLotData = new ScalesAutomation.LotData();
            this.ShowNextLotData = new System.Windows.Forms.Button();
            this.toolTipPrepareNextLot = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStart.Location = new System.Drawing.Point(332, 371);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(200, 77);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPause.Location = new System.Drawing.Point(576, 371);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(200, 77);
            this.btnPause.TabIndex = 8;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // dataGridViewMeasurements
            // 
            this.dataGridViewMeasurements.AllowUserToAddRows = false;
            this.dataGridViewMeasurements.AllowUserToDeleteRows = false;
            this.dataGridViewMeasurements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewMeasurements.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewMeasurements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMeasurements.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewMeasurements.Location = new System.Drawing.Point(990, 15);
            this.dataGridViewMeasurements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridViewMeasurements.Name = "dataGridViewMeasurements";
            this.dataGridViewMeasurements.ReadOnly = true;
            this.dataGridViewMeasurements.RowHeadersWidth = 10;
            this.dataGridViewMeasurements.RowTemplate.Height = 25;
            this.dataGridViewMeasurements.Size = new System.Drawing.Size(320, 600);
            this.dataGridViewMeasurements.TabIndex = 3;
            this.dataGridViewMeasurements.TabStop = false;
            // 
            // btnStopLot
            // 
            this.btnStopLot.Enabled = false;
            this.btnStopLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStopLot.Location = new System.Drawing.Point(449, 478);
            this.btnStopLot.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStopLot.Name = "btnStopLot";
            this.btnStopLot.Size = new System.Drawing.Size(200, 77);
            this.btnStopLot.TabIndex = 9;
            this.btnStopLot.Text = "Stop Lot";
            this.btnStopLot.UseVisualStyleBackColor = true;
            this.btnStopLot.Click += new System.EventHandler(this.btnStopLot_Click);
            // 
            // uctlLotData
            // 
            this.uctlLotData.Location = new System.Drawing.Point(5, 5);
            this.uctlLotData.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.uctlLotData.Name = "uctlLotData";
            this.uctlLotData.Size = new System.Drawing.Size(980, 329);
            this.uctlLotData.TabIndex = 0;
            // 
            // ShowNextLotData
            // 
            this.ShowNextLotData.Font = new System.Drawing.Font("Consolas", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ShowNextLotData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.ShowNextLotData.Location = new System.Drawing.Point(47, 371);
            this.ShowNextLotData.Margin = new System.Windows.Forms.Padding(4);
            this.ShowNextLotData.Name = "ShowNextLotData";
            this.ShowNextLotData.Size = new System.Drawing.Size(70, 70);
            this.ShowNextLotData.TabIndex = 10;
            this.ShowNextLotData.Text = "+";
            this.toolTipPrepareNextLot.SetToolTip(this.ShowNextLotData, "Pregateste datele pentru urmatorul Lot");
            this.ShowNextLotData.UseVisualStyleBackColor = true;
            this.ShowNextLotData.Click += new System.EventHandler(this.btnShowNextLotData_Click);
            // 
            // ScalesAutomationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 633);
            this.Controls.Add(this.ShowNextLotData);
            this.Controls.Add(this.uctlLotData);
            this.Controls.Add(this.btnStopLot);
            this.Controls.Add(this.dataGridViewMeasurements);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScalesAutomationForm";
            this.Text = "Automatizare Cantar Bilanciai";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScalesAutomation_FormClosing);
            this.Load += new System.EventHandler(this.ScalesAutomation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeasurements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.DataGridView dataGridViewMeasurements;
        private System.Windows.Forms.BindingSource measurementBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource1;
        private System.Windows.Forms.BindingSource measurementBindingSource1;
        private System.Windows.Forms.Button btnStopLot;
        private LotData uctlLotData;
        private System.Windows.Forms.Button ShowNextLotData;
        private System.Windows.Forms.ToolTip toolTipPrepareNextLot;
    }
}

