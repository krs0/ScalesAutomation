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
            this.btnPause = new System.Windows.Forms.Button();
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
            this.cbProduct = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scalesAutomationBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.measurementBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(99, 293);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 50);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(282, 293);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(150, 50);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // dataGridViewMeasurements
            // 
            this.dataGridViewMeasurements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMeasurements.Location = new System.Drawing.Point(501, 24);
            this.dataGridViewMeasurements.Name = "dataGridViewMeasurements";
            this.dataGridViewMeasurements.Size = new System.Drawing.Size(267, 426);
            this.dataGridViewMeasurements.TabIndex = 3;
            // 
            // chkEnableSimulation
            // 
            this.chkEnableSimulation.AutoSize = true;
            this.chkEnableSimulation.Checked = true;
            this.chkEnableSimulation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableSimulation.Location = new System.Drawing.Point(28, 435);
            this.chkEnableSimulation.Name = "chkEnableSimulation";
            this.chkEnableSimulation.Size = new System.Drawing.Size(110, 17);
            this.chkEnableSimulation.TabIndex = 4;
            this.chkEnableSimulation.Text = "Enable Simulation";
            this.chkEnableSimulation.UseVisualStyleBackColor = true;
            this.chkEnableSimulation.CheckedChanged += new System.EventHandler(this.chkEnableSimulation_CheckedChanged);
            // 
            // txtLot
            // 
            this.txtLot.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLot.Location = new System.Drawing.Point(184, 24);
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new System.Drawing.Size(288, 26);
            this.txtLot.TabIndex = 5;
            this.txtLot.Validated += new System.EventHandler(this.txtLot_Validated);
            // 
            // txtNominalWeight
            // 
            this.txtNominalWeight.Enabled = false;
            this.txtNominalWeight.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNominalWeight.Location = new System.Drawing.Point(184, 192);
            this.txtNominalWeight.Name = "txtNominalWeight";
            this.txtNominalWeight.Size = new System.Drawing.Size(288, 26);
            this.txtNominalWeight.TabIndex = 6;
            this.txtNominalWeight.Validated += new System.EventHandler(this.txtNominalWeight_Validated);
            // 
            // txtPackageTare
            // 
            this.txtPackageTare.Enabled = false;
            this.txtPackageTare.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPackageTare.Location = new System.Drawing.Point(184, 150);
            this.txtPackageTare.Name = "txtPackageTare";
            this.txtPackageTare.Size = new System.Drawing.Size(288, 26);
            this.txtPackageTare.TabIndex = 7;
            this.txtPackageTare.Validated += new System.EventHandler(this.txtPackageTare_Validated);
            // 
            // cbPackage
            // 
            this.cbPackage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPackage.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPackage.FormattingEnabled = true;
            this.cbPackage.Location = new System.Drawing.Point(184, 108);
            this.cbPackage.Margin = new System.Windows.Forms.Padding(2);
            this.cbPackage.Name = "cbPackage";
            this.cbPackage.Size = new System.Drawing.Size(288, 26);
            this.cbPackage.TabIndex = 8;
            this.cbPackage.SelectedIndexChanged += new System.EventHandler(this.cbPackage_SelectedIndexChanged);
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProduct.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblProduct.Location = new System.Drawing.Point(25, 70);
            this.lblProduct.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(115, 18);
            this.lblProduct.TabIndex = 9;
            this.lblProduct.Text = "Sortimentul:";
            // 
            // lblLot
            // 
            this.lblLot.AutoSize = true;
            this.lblLot.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLot.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblLot.Location = new System.Drawing.Point(25, 28);
            this.lblLot.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLot.Name = "lblLot";
            this.lblLot.Size = new System.Drawing.Size(42, 18);
            this.lblLot.TabIndex = 10;
            this.lblLot.Text = "Lot:";
            // 
            // lblNominalWeight
            // 
            this.lblNominalWeight.AutoSize = true;
            this.lblNominalWeight.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNominalWeight.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblNominalWeight.Location = new System.Drawing.Point(24, 196);
            this.lblNominalWeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNominalWeight.Name = "lblNominalWeight";
            this.lblNominalWeight.Size = new System.Drawing.Size(143, 18);
            this.lblNominalWeight.TabIndex = 11;
            this.lblNominalWeight.Text = "Masa Nominala:";
            // 
            // lblPackage
            // 
            this.lblPackage.AutoSize = true;
            this.lblPackage.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPackage.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblPackage.Location = new System.Drawing.Point(24, 112);
            this.lblPackage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPackage.Name = "lblPackage";
            this.lblPackage.Size = new System.Drawing.Size(116, 18);
            this.lblPackage.TabIndex = 12;
            this.lblPackage.Text = "Tip Ambalaj:";
            // 
            // lblPackageTare
            // 
            this.lblPackageTare.AutoSize = true;
            this.lblPackageTare.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPackageTare.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblPackageTare.Location = new System.Drawing.Point(24, 154);
            this.lblPackageTare.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPackageTare.Name = "lblPackageTare";
            this.lblPackageTare.Size = new System.Drawing.Size(127, 18);
            this.lblPackageTare.TabIndex = 13;
            this.lblPackageTare.Text = "Tara Ambalaj:";
            // 
            // cbProduct
            // 
            this.cbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProduct.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbProduct.FormattingEnabled = true;
            this.cbProduct.Location = new System.Drawing.Point(184, 66);
            this.cbProduct.Margin = new System.Windows.Forms.Padding(2);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Size = new System.Drawing.Size(288, 26);
            this.cbProduct.TabIndex = 14;
            this.cbProduct.SelectedIndexChanged += new System.EventHandler(this.cbProduct_SelectedIndexChanged);
            // 
            // ScalesAutomation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 465);
            this.Controls.Add(this.cbProduct);
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
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStart);
            this.Name = "ScalesAutomation";
            this.Text = "Automatizare Cantar Bilanciai";
            this.Load += new System.EventHandler(this.ScalesAutomation_Load);
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
        private System.Windows.Forms.Button btnPause;
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
        private System.Windows.Forms.ComboBox cbProduct;
    }
}

