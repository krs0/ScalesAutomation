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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.btnShowNextLotData = new System.Windows.Forms.Button();
            this.toolTipPrepareNextLot = new System.Windows.Forms.ToolTip(this.components);
            this.btnDeleteLastMeasurement = new System.Windows.Forms.Button();
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
            this.btnStart.Location = new System.Drawing.Point(329, 457);
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
            this.btnPause.Location = new System.Drawing.Point(573, 457);
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
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewMeasurements.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewMeasurements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMeasurements.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewMeasurements.Location = new System.Drawing.Point(990, 15);
            this.dataGridViewMeasurements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridViewMeasurements.MultiSelect = false;
            this.dataGridViewMeasurements.Name = "dataGridViewMeasurements";
            this.dataGridViewMeasurements.ReadOnly = true;
            this.dataGridViewMeasurements.RowHeadersWidth = 10;
            this.dataGridViewMeasurements.RowTemplate.Height = 25;
            this.dataGridViewMeasurements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewMeasurements.Size = new System.Drawing.Size(320, 725);
            this.dataGridViewMeasurements.TabIndex = 3;
            this.dataGridViewMeasurements.TabStop = false;
            // 
            // btnStopLot
            // 
            this.btnStopLot.Enabled = false;
            this.btnStopLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStopLot.Location = new System.Drawing.Point(446, 564);
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
            // btnShowNextLotData
            // 
            this.btnShowNextLotData.Font = new System.Drawing.Font("Consolas", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnShowNextLotData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnShowNextLotData.Location = new System.Drawing.Point(46, 368);
            this.btnShowNextLotData.Margin = new System.Windows.Forms.Padding(4);
            this.btnShowNextLotData.Name = "btnShowNextLotData";
            this.btnShowNextLotData.Size = new System.Drawing.Size(70, 70);
            this.btnShowNextLotData.TabIndex = 10;
            this.btnShowNextLotData.Text = "+";
            this.toolTipPrepareNextLot.SetToolTip(this.btnShowNextLotData, "Pregateste datele pentru urmatorul Lot");
            this.btnShowNextLotData.UseVisualStyleBackColor = true;
            this.btnShowNextLotData.Click += new System.EventHandler(this.btnShowNextLotData_Click);
            // 
            // btnDeleteLastMeasurement
            // 
            this.btnDeleteLastMeasurement.Font = new System.Drawing.Font("Consolas", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDeleteLastMeasurement.ForeColor = System.Drawing.Color.Red;
            this.btnDeleteLastMeasurement.Location = new System.Drawing.Point(912, 670);
            this.btnDeleteLastMeasurement.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteLastMeasurement.Name = "btnDeleteLastMeasurement";
            this.btnDeleteLastMeasurement.Size = new System.Drawing.Size(70, 70);
            this.btnDeleteLastMeasurement.TabIndex = 11;
            this.btnDeleteLastMeasurement.Text = "X";
            this.toolTipPrepareNextLot.SetToolTip(this.btnDeleteLastMeasurement, "Sterge ultima masuratoare!");
            this.btnDeleteLastMeasurement.UseVisualStyleBackColor = true;
            this.btnDeleteLastMeasurement.Click += new System.EventHandler(this.btnDeleteLastMeasurement_Click);
            // 
            // ScalesAutomationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 753);
            this.Controls.Add(this.btnDeleteLastMeasurement);
            this.Controls.Add(this.btnShowNextLotData);
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
        private System.Windows.Forms.Button btnShowNextLotData;
        private System.Windows.Forms.ToolTip toolTipPrepareNextLot;
        private System.Windows.Forms.Button btnDeleteLastMeasurement;
    }
}

