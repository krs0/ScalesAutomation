using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using ScalesAutomation.Properties;
using System.IO;

namespace ScalesAutomation
{
    public partial class LotData : UserControl
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        XmlHelper XmlHandler = new XmlHelper();

        #region Properties

        public LotInfo LotInfo { get; private set; }
        Product ProductDefinition { get; set; }
        Package PackageDefinition { get; set; }

        #endregion

        public LotData()
        {
            InitializeComponent();

            var catalogFilePath = Settings.Default.CatalogFilePath;
            if (!catalogFilePath.Contains(":")) // if relative file path
                catalogFilePath = Path.Combine(Misc.AssemblyPath, catalogFilePath);

            XmlHandler.ReadCatalogue(catalogFilePath);

            InitializeGuiBackendFromXml();

            LotInfo = new LotInfo();
        }

        #region "Events For Input Controls"

        void txtLot_Validated(object sender, EventArgs e)
        {
            var logFilePath = "";
            var outputFilePath = "";
            var outputFolderPath = Path.Combine(Misc.AssemblyPath, Settings.Default.CSVServerFolderPath);
            var logFolderPath = Path.Combine(Misc.AssemblyPath, Settings.Default.LogFolderPath);

            if (txtLot.Text == "")
                return;

            if (!Settings.Default.DataImporterEnabled)
            {
                // Check if measurements were already done for selected lot
                if (CsvHelper.LogAlreadyPresent(txtLot.Text, logFolderPath, ref logFilePath))
                {
                    DialogResult result = MessageBox.Show("Pentru lotul selectat exista deja masuratori. Doriti sa continuati lotul?", "Continuare Lot", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        var lotInfo = LotInfo.ReadLotInfo(logFilePath);
                        lotInfo.AppendToLot = true;
                        SetLotInfo(lotInfo);
                    }
                    else
                    {
                        txtLot.Text = "";
                        LotInfo.Lot = "";
                        LotInfo.AppendToLot = false;
                    }
                }
                else
                {
                    LotInfo.Lot = txtLot.Text;
                }
            }
            else
            {
                if (CsvHelper.LotAlreadyPresent(txtLot.Text, outputFolderPath, ref outputFilePath))
                {
                    DialogResult result = MessageBox.Show("Pentru lotul selectat exista deja masuratori. Doriti sa continuati lotul?" + Environment.NewLine + "(Masuratorile vor fi adaugate in continuare la cele existente pentru acest lot!)", "Continuare Lot", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        var lotInfo = CsvHelper.ReadMeasurementFileHeader(outputFilePath);
                        lotInfo.AppendToLot = true;
                        SetLotInfo(lotInfo);
                    }
                    else
                    {
                        txtLot.Text = "";
                        LotInfo.Lot = "";
                        LotInfo.AppendToLot = false;
                    }

                }
                else
                {
                    LotInfo.Lot = txtLot.Text;                        
                }
            }

            if (LotInfo.AppendToLot)
            {
                DisableInputControls();
                txtLot.Enabled = true;
            }
            else
            {
                EnableInputControls();
            }
        }

        void txtLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                SelectNextControl(this.ActiveControl, true, true, true, true);
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
        }

        void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPackage.SelectedIndex == -1) return;

            // TODO: Separate type and netweight by _
            LotInfo.Package.Type = cbPackage.Text;

            PackageDefinition = ProductDefinition.PackageDetails.Find(x => x.Type == LotInfo.Package.Type);
            LotInfo.Package.Tare = PackageDefinition.Tare;
            LotInfo.Package.NetWeight = PackageDefinition.NetWeight;
            LotInfo.Package.TotalWeight = PackageDefinition.TotalWeight;

            txtPackageTare.Text = LotInfo.Package.Tare + "Kg";
            txtNominalWeight.Text = LotInfo.Package.TotalWeight + "Kg";
        }

        void txtPackageTare_Validated(object sender, EventArgs e)
        {
            if (txtPackageTare.Text.IndexOf("Kg", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                MessageBox.Show("Tara trebuie sa fie urmata de unitatea de masura. Ex: 20.5Kg" + Environment.NewLine + "Doar Kg sunt suportate ca unitate de masura!");
                return;
            }

            Double.TryParse(Regex.Replace(txtPackageTare.Text, "Kg", "", RegexOptions.IgnoreCase), out LotInfo.Package.Tare);
            LotInfo.Package.TotalWeight = LotInfo.Package.NetWeight + LotInfo.Package.Tare;
            txtNominalWeight.Text = LotInfo.Package.TotalWeight + "Kg";
        }

        void txtPackageTare_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
        }

        #endregion

        public void SetLotInfo(LotInfo lotInfo)
        {
            LotInfo = lotInfo ?? throw new ArgumentNullException(nameof(lotInfo));

            txtLot.Text = LotInfo.Lot;
            cbProduct.SelectedItem = LotInfo.ProductName;
            cbPackage.SelectedItem = LotInfo.Package.Type;
            txtPackageTare.Text = (LotInfo.Package.Tare != 0) ? LotInfo.Package.Tare + "Kg" : "";

            // Tare validated() event will fill Nominal Weight

        }

        // double make sure we do noy use an old object
        public void InitializeLotInfo()
        {
            LotInfo = new LotInfo();
        }

        public bool CheckInputControls()
        {
            bool inputsAreValid = true;

            if ((txtLot.Text == "") || (cbProduct.SelectedIndex == -1) || (cbPackage.SelectedIndex == -1) || (txtPackageTare.Text == "") || (txtNominalWeight.Text == ""))
            {
                log.Info("Invalid Lot configuration detected!" + Environment.NewLine);
                MessageBox.Show("Configuratie de Lot invalida! Asigurati-va ca toate campurile sunt completate si au valori corecte.",
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
            txtNominalWeight.Enabled = false;
        }

        public void DisableInputControls()
        {
            txtLot.Enabled = false;
            cbProduct.Enabled = false;
            cbPackage.Enabled = false;
            txtPackageTare.Enabled = false;
            txtNominalWeight.Enabled = false;
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
            txtNominalWeight.Text = "";

            InitializeLotInfo();
        }

        void InitializeGuiBackendFromXml()
        {
            foreach (var product in XmlHandler.Catalogue)
            {
                cbProduct.Items.Add(product.Name);
            }
        }
    }
}
