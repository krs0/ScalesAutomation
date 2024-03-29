using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using ScalesAutomation.Properties;
using System.IO;
using CommonNS;

namespace ScalesAutomation
{
    public partial class LotData : UserControl
    {
        readonly ILog log = LogManager.GetLogger("generalLog");

        XmlHelper XmlHandler = new XmlHelper();

        #region Properties

        public LotInfo LotInfo { get; private set; }
        Product ProductDefinition { get; set; }
        Package PackageDefinition { get; set; }

        #endregion

        public LotData()
        {
            InitializeComponent();

            InitializeGuiBackendFromXml();

            InitializeLotInfo();
        }

        #region "Events For Input Controls"

        void txtLot_Validated(object sender, EventArgs e)
        {
            LotInfo.Lot = txtLot.Text;
        }

        void txtLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnEnterSelectNextControl(e);
        }

        void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProduct.SelectedIndex == -1) return;

            LotInfo.ProductName = cbProduct.Text;
            ProductDefinition = XmlHandler.Catalogue.Find(x => x.Name == LotInfo.ProductName);

            cbPackage.Items.Clear();
            foreach (var package in ProductDefinition.PackageDetails)
            {
                cbPackage.Items.Add(package.Type);
            }

            cbPackage.SelectedIndex = -1;
            txtPackageTare.Text = "";
            txtNominalWeight.Text = "";

            // if only one package select it automatically and also its Tare will be automatically selected.
            if (cbPackage.Items.Count == 1)
                cbPackage.SelectedIndex = 0;
        }

        void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPackage.SelectedIndex == -1) return;

            LotInfo.Package.Type = cbPackage.Text;

            PackageDefinition = ProductDefinition.PackageDetails.Find(x => x.Type == LotInfo.Package.Type);
            LotInfo.Package.Tare = PackageDefinition.Tare;
            LotInfo.TareIsSet = cbPackageTareSet.Checked;
            LotInfo.Package.NetWeight = PackageDefinition.NetWeight;
            LotInfo.Package.TotalWeight = PackageDefinition.TotalWeight;

            txtPackageTare.Text = LotInfo.Package.Tare + "g";
            txtNominalWeight.Text = LotInfo.Package.TotalWeight / 1000 + "Kg";
        }

        void txtPackageTare_Validated(object sender, EventArgs e)
        {
            LotInfo.Package.Tare = Misc.GetValueInGrams(txtPackageTare.Text);
            LotInfo.Package.TotalWeight = LotInfo.Package.NetWeight + LotInfo.Package.Tare;
            txtNominalWeight.Text = LotInfo.Package.TotalWeight / 1000 + "Kg";
        }

        void txtPackageTare_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnEnterSelectNextControl(e);
        }

        private void cbPackageTareSet_CheckStateChanged(object sender, EventArgs e)
        {
            LotInfo.TareIsSet = cbPackageTareSet.Checked;
        }

        private void OnEnterSelectNextControl(KeyPressEventArgs e)
        {
            if ((e.KeyChar == 13) && (txtLot.Text != ""))
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
        }

        #endregion

        // Make sure we do noy use an old object
        public void InitializeLotInfo()
        {
            LotInfo = new LotInfo();
        }

        public void SetLotInfo(LotInfo lotInfo)
        {
            LotInfo = lotInfo ?? throw new ArgumentNullException(nameof(lotInfo));

            txtLot.Text = LotInfo.Lot;
            cbProduct.SelectedItem = LotInfo.ProductName;
            cbPackage.SelectedItem = LotInfo.Package.Type;
            txtPackageTare.Text = (LotInfo.Package.Tare != 0) ? LotInfo.Package.Tare.ToString() : "";
            cbPackageTareSet.Checked = LotInfo.TareIsSet;

            // Tare validated() event will fill Nominal Weight
        }

        public bool AreInputControlsValid()
        {
            bool inputsAreValid = true;

            if ((txtLot.Text == "") || (cbProduct.SelectedIndex == -1) || (cbPackage.SelectedIndex == -1) || (txtPackageTare.Text == "") || (txtNominalWeight.Text == ""))
            {
                log.Info($"Invalid Lot configuration detected!{Environment.NewLine}");
                MessageBox.Show("Configuratie de Lot invalida! Asigurati-va ca toate campurile sunt completate si au valori corecte.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputsAreValid = false;
            }

            if (!PathHelper.IsPathValid(txtLot.Text))
            {
                log.Info($"Lot Name {txtLot.Text} contains invalid characters!{Environment.NewLine}");
                MessageBox.Show("Numele lotului contine caractere invalide! Asigurati-va ca nu contine caractere speciale ca: \", *, /, :, <, >, ?, \\, |",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputsAreValid = false;
            }

            if(!PathHelper.IsPathValid(cbProduct.Text))
            {
                log.Info($"Product Name {cbProduct.Text} contains invalid characters!{Environment.NewLine}");
                MessageBox.Show("Numele Sortimentului contine caractere invalide! Asigurati-va ca nu contine caractere speciale ca: \", *, /, :, <, >, ?, \\, |",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputsAreValid = false;
            }

            return inputsAreValid;
        }

        public void EnableInputControls()
        {
            txtLot.Enabled = true;
            cbProduct.Enabled = true;
            cbPackage.Enabled = true;
            txtPackageTare.Enabled = true;
            cbPackageTareSet.Enabled = true;
            txtNominalWeight.Enabled = false;
        }

        public void DisableLotControls()
        {
            txtLot.Enabled = false;
            cbProduct.Enabled = false;
            cbPackage.Enabled = false;
            txtPackageTare.Enabled = false;
            txtNominalWeight.Enabled = false;
            cbPackageTareSet.Enabled = false;
        }

        public bool InputControlsEnabled()
        {
            return txtLot.Enabled;
        }

        public void InitializeInputControls()
        {
            txtLot.Text = "";
            cbProduct.SelectedIndex = -1;
            cbPackage.SelectedIndex = -1;
            txtPackageTare.Text = "";
            cbPackageTareSet.Checked = true;
            txtNominalWeight.Text = "";

            InitializeLotInfo();
        }

        void InitializeGuiBackendFromXml()
        {
            string productCatalogFilePath = Common.TransformToAbsolutePath(Settings.Default.ProductCatalogFilePath);
            try
            {
                XmlHandler.ReadCatalogue(productCatalogFilePath);
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Eorare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }


            foreach(var product in XmlHandler.Catalogue)
            {
                cbProduct.Items.Add(product.Name);
            }
        }
    }
}
