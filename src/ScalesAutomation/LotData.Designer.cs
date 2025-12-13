namespace ScalesAutomation
{
    partial class LotData
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbProduct = new System.Windows.Forms.ComboBox();
            this.lblPackageTare = new System.Windows.Forms.Label();
            this.lblPackage = new System.Windows.Forms.Label();
            this.lblNominalWeight = new System.Windows.Forms.Label();
            this.lblLot = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cbPackage = new System.Windows.Forms.ComboBox();
            this.txtPackageTare = new System.Windows.Forms.TextBox();
            this.txtNominalWeight = new System.Windows.Forms.TextBox();
            this.txtLot = new System.Windows.Forms.TextBox();
            this.cbPackageTareSet = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbProduct
            // 
            this.cbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProduct.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbProduct.FormattingEnabled = true;
            this.cbProduct.Location = new System.Drawing.Point(180, 80);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Size = new System.Drawing.Size(790, 32);
            this.cbProduct.TabIndex = 2;
            this.cbProduct.SelectedIndexChanged += new System.EventHandler(this.cbProduct_SelectedIndexChanged);
            // 
            // lblPackageTare
            // 
            this.lblPackageTare.AutoSize = true;
            this.lblPackageTare.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPackageTare.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblPackageTare.Location = new System.Drawing.Point(5, 209);
            this.lblPackageTare.Name = "lblPackageTare";
            this.lblPackageTare.Size = new System.Drawing.Size(156, 24);
            this.lblPackageTare.TabIndex = 24;
            this.lblPackageTare.Text = "Tara Ambalaj:";
            // 
            // lblPackage
            // 
            this.lblPackage.AutoSize = true;
            this.lblPackage.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPackage.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblPackage.Location = new System.Drawing.Point(5, 145);
            this.lblPackage.Name = "lblPackage";
            this.lblPackage.Size = new System.Drawing.Size(142, 24);
            this.lblPackage.TabIndex = 23;
            this.lblPackage.Text = "Tip Ambalaj:";
            // 
            // lblNominalWeight
            // 
            this.lblNominalWeight.AutoSize = true;
            this.lblNominalWeight.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblNominalWeight.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblNominalWeight.Location = new System.Drawing.Point(5, 282);
            this.lblNominalWeight.Name = "lblNominalWeight";
            this.lblNominalWeight.Size = new System.Drawing.Size(142, 24);
            this.lblNominalWeight.TabIndex = 25;
            this.lblNominalWeight.Text = "Masa Totala:";
            // 
            // lblLot
            // 
            this.lblLot.AutoSize = true;
            this.lblLot.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLot.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblLot.Location = new System.Drawing.Point(7, 15);
            this.lblLot.Name = "lblLot";
            this.lblLot.Size = new System.Drawing.Size(52, 24);
            this.lblLot.TabIndex = 21;
            this.lblLot.Text = "Lot:";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblProduct.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblProduct.Location = new System.Drawing.Point(7, 80);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(142, 24);
            this.lblProduct.TabIndex = 22;
            this.lblProduct.Text = "Sortimentul:";
            // 
            // cbPackage
            // 
            this.cbPackage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPackage.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbPackage.FormattingEnabled = true;
            this.cbPackage.Location = new System.Drawing.Point(180, 145);
            this.cbPackage.Name = "cbPackage";
            this.cbPackage.Size = new System.Drawing.Size(790, 32);
            this.cbPackage.TabIndex = 3;
            this.cbPackage.SelectedIndexChanged += new System.EventHandler(this.cbPackage_SelectedIndexChanged);
            // 
            // txtPackageTare
            // 
            this.txtPackageTare.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtPackageTare.ForeColor = System.Drawing.Color.OrangeRed;
            this.txtPackageTare.Location = new System.Drawing.Point(180, 209);
            this.txtPackageTare.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPackageTare.Name = "txtPackageTare";
            this.txtPackageTare.Size = new System.Drawing.Size(790, 30);
            this.txtPackageTare.TabIndex = 4;
            this.txtPackageTare.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPackageTare_KeyPress);
            this.txtPackageTare.Validated += new System.EventHandler(this.txtPackageTare_Validated);
            // 
            // txtNominalWeight
            // 
            this.txtNominalWeight.Enabled = false;
            this.txtNominalWeight.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtNominalWeight.Location = new System.Drawing.Point(180, 279);
            this.txtNominalWeight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNominalWeight.Name = "txtNominalWeight";
            this.txtNominalWeight.Size = new System.Drawing.Size(790, 30);
            this.txtNominalWeight.TabIndex = 6;
            // 
            // txtLot
            // 
            this.txtLot.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtLot.Location = new System.Drawing.Point(180, 15);
            this.txtLot.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new System.Drawing.Size(790, 30);
            this.txtLot.TabIndex = 1;
            this.txtLot.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLot_KeyPress);
            this.txtLot.Validated += new System.EventHandler(this.txtLot_Validated);
            // 
            // cbPackageTareSet
            // 
            this.cbPackageTareSet.AutoSize = true;
            this.cbPackageTareSet.Checked = true;
            this.cbPackageTareSet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPackageTareSet.Location = new System.Drawing.Point(180, 247);
            this.cbPackageTareSet.Name = "cbPackageTareSet";
            this.cbPackageTareSet.Size = new System.Drawing.Size(186, 24);
            this.cbPackageTareSet.TabIndex = 5;
            this.cbPackageTareSet.Text = "Cantarul are Tara setata";
            this.cbPackageTareSet.UseVisualStyleBackColor = true;
            this.cbPackageTareSet.CheckStateChanged += new System.EventHandler(this.cbPackageTareSet_CheckStateChanged);
            // 
            // LotData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbPackageTareSet);
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
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "LotData";
            this.Size = new System.Drawing.Size(990, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbProduct;
        private System.Windows.Forms.Label lblPackageTare;
        private System.Windows.Forms.Label lblPackage;
        private System.Windows.Forms.Label lblNominalWeight;
        private System.Windows.Forms.Label lblLot;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cbPackage;
        private System.Windows.Forms.TextBox txtPackageTare;
        private System.Windows.Forms.TextBox txtNominalWeight;
        private System.Windows.Forms.TextBox txtLot;
        private System.Windows.Forms.CheckBox cbPackageTareSet;
    }
}
