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
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.dataGridViewMeasurements = new System.Windows.Forms.DataGridView();
            this.measurementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scalesAutomationBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.measurementBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.chkEnableSimulation = new System.Windows.Forms.CheckBox();
            this.txtLot = new System.Windows.Forms.TextBox();
            this.txtNominalWeight = new System.Windows.Forms.TextBox();
            this.txtPackageTare = new System.Windows.Forms.TextBox();
            this.cbPackage = new System.Windows.Forms.ComboBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblLot = new System.Windows.Forms.Label();
            this.lblNominalWeight = new System.Windows.Forms.Label();
            this.lblPackage = new System.Windows.Forms.Label();
            this.lblPackageTare = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(64, 478);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 35);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(238, 478);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(112, 35);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(191, 40);
            this.txtProduct.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(159, 26);
            this.txtProduct.TabIndex = 2;
            this.txtProduct.Validated += new System.EventHandler(this.txtProduct_Validated);
            // 
            // dataGridViewMeasurements
            // 
            this.dataGridViewMeasurements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMeasurements.Location = new System.Drawing.Point(386, 40);
            this.dataGridViewMeasurements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridViewMeasurements.Name = "dataGridViewMeasurements";
            this.dataGridViewMeasurements.Size = new System.Drawing.Size(270, 656);
            this.dataGridViewMeasurements.TabIndex = 3;
            // 
            // chkEnableSimulation
            // 
            this.chkEnableSimulation.AutoSize = true;
            this.chkEnableSimulation.Checked = true;
            this.chkEnableSimulation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableSimulation.Location = new System.Drawing.Point(64, 672);
            this.chkEnableSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkEnableSimulation.Name = "chkEnableSimulation";
            this.chkEnableSimulation.Size = new System.Drawing.Size(163, 24);
            this.chkEnableSimulation.TabIndex = 4;
            this.chkEnableSimulation.Text = "Enable Simulation";
            this.chkEnableSimulation.UseVisualStyleBackColor = true;
            this.chkEnableSimulation.CheckedChanged += new System.EventHandler(this.chkEnableSimulation_CheckedChanged);
            // 
            // txtLot
            // 
            this.txtLot.Location = new System.Drawing.Point(191, 93);
            this.txtLot.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new System.Drawing.Size(159, 26);
            this.txtLot.TabIndex = 5;
            this.txtLot.Validated += new System.EventHandler(this.txtLot_Validated);
            // 
            // txtNominalWeight
            // 
            this.txtNominalWeight.Location = new System.Drawing.Point(191, 146);
            this.txtNominalWeight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNominalWeight.Name = "txtNominalWeight";
            this.txtNominalWeight.Size = new System.Drawing.Size(159, 26);
            this.txtNominalWeight.TabIndex = 6;
            this.txtNominalWeight.Validated += new System.EventHandler(this.txtNominalWeight_Validated);
            // 
            // txtPackageTare
            // 
            this.txtPackageTare.Location = new System.Drawing.Point(191, 254);
            this.txtPackageTare.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPackageTare.Name = "txtPackageTare";
            this.txtPackageTare.Size = new System.Drawing.Size(159, 26);
            this.txtPackageTare.TabIndex = 7;
            this.txtPackageTare.Validated += new System.EventHandler(this.txtPackageTare_Validated);
            // 
            // cbPackage
            // 
            this.cbPackage.FormattingEnabled = true;
            this.cbPackage.Location = new System.Drawing.Point(191, 199);
            this.cbPackage.Name = "cbPackage";
            this.cbPackage.Size = new System.Drawing.Size(159, 28);
            this.cbPackage.TabIndex = 8;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(36, 45);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(94, 20);
            this.lblProduct.TabIndex = 9;
            this.lblProduct.Text = "Sortimentul:";
            // 
            // lblLot
            // 
            this.lblLot.AutoSize = true;
            this.lblLot.Location = new System.Drawing.Point(36, 99);
            this.lblLot.Name = "lblLot";
            this.lblLot.Size = new System.Drawing.Size(36, 20);
            this.lblLot.TabIndex = 10;
            this.lblLot.Text = "Lot:";
            // 
            // lblNominalWeight
            // 
            this.lblNominalWeight.AutoSize = true;
            this.lblNominalWeight.Location = new System.Drawing.Point(36, 153);
            this.lblNominalWeight.Name = "lblNominalWeight";
            this.lblNominalWeight.Size = new System.Drawing.Size(122, 20);
            this.lblNominalWeight.TabIndex = 11;
            this.lblNominalWeight.Text = "Masa Nominala:";
            // 
            // lblPackage
            // 
            this.lblPackage.AutoSize = true;
            this.lblPackage.Location = new System.Drawing.Point(36, 207);
            this.lblPackage.Name = "lblPackage";
            this.lblPackage.Size = new System.Drawing.Size(95, 20);
            this.lblPackage.TabIndex = 12;
            this.lblPackage.Text = "Tip Ambalaj:";
            // 
            // lblPackageTare
            // 
            this.lblPackageTare.AutoSize = true;
            this.lblPackageTare.Location = new System.Drawing.Point(36, 261);
            this.lblPackageTare.Name = "lblPackageTare";
            this.lblPackageTare.Size = new System.Drawing.Size(106, 20);
            this.lblPackageTare.TabIndex = 13;
            this.lblPackageTare.Text = "Tara Ambalaj:";
            // 
            // ScalesAutomation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 715);
            this.Controls.Add(this.lblPackageTare);
            this.Controls.Add(this.lblPackage);
            this.Controls.Add(this.lblNominalWeight);
            this.Controls.Add(this.lblLot);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.cbPackage);
            this.Controls.Add(this.txtPackageTare);
            this.Controls.Add(this.txtNominalWeight);
            this.Controls.Add(this.txtLot);
            this.Controls.Add(this.chkEnableSimulation);
            this.Controls.Add(this.dataGridViewMeasurements);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScalesAutomation";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeasurements)).EndInit();
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
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.DataGridView dataGridViewMeasurements;
        private System.Windows.Forms.BindingSource measurementBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource;
        private System.Windows.Forms.BindingSource scalesAutomationBindingSource1;
        private System.Windows.Forms.BindingSource measurementBindingSource1;
        private System.Windows.Forms.CheckBox chkEnableSimulation;
        private System.Windows.Forms.TextBox txtLot;
        private System.Windows.Forms.TextBox txtNominalWeight;
        private System.Windows.Forms.TextBox txtPackageTare;
        private System.Windows.Forms.ComboBox cbPackage;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblLot;
        private System.Windows.Forms.Label lblNominalWeight;
        private System.Windows.Forms.Label lblPackage;
        private System.Windows.Forms.Label lblPackageTare;
    }
}

