using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScalesAutomation.Properties;

namespace ScalesAutomation
{
    public partial class DataImporter : Form
    {
        public DataImporter()
        {
            InitializeComponent();
        }

        private void dateTimePicker_ValueChanged(object sender, System.EventArgs e)
        {
            var lotInfo = uctlLotData.LotInfo;
            lotInfo.Date = dateTimePicker.Value.Date.ToString("yyyy-MM-dd");

            uctlLotData.SetLotInfo(lotInfo);
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog.FileName;
            }
        }

        private void btnImport_Click(object sender, System.EventArgs e)
        {

            var outputFolderPath = Settings.Default.DataImporterOutputPath;
            var outputFilePath = "";
            var startMeasurementIndex = 1;

            if (!uctlLotData.AreInputControlsValid())
                return;

            var inputFilePath = txtFileName.Text;

            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Nu s-a specificat un fisier cu date de intrare!", "Date Incorecte", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var lotInfo = uctlLotData.LotInfo;

                var intermediateFilePath = Path.GetDirectoryName(inputFilePath) + "\\temp_.bak";

                if (CsvHelper.OutputAlreadyPresent(lotInfo.Id, outputFolderPath, ref outputFilePath))
                {
                    var result = MessageBox.Show("Pentru lotul selectat exista deja masuratori. Noile masuratori se vor adauga celor existente. Doriti sa Continuati?", "Continuare Lot", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                        return;

                    lotInfo.AppendToLot = true;
                }

                if (lotInfo.AppendToLot)
                {
                    var lastMeasurementIndex = CsvHelper.GetLastMeasurementIndex(outputFilePath);
                    startMeasurementIndex = Int32.Parse(lastMeasurementIndex) + 1;
                }
                else
                {
                    outputFilePath = CsvHelper.CalculateOutputFilePath(outputFolderPath, DateTime.Now, lotInfo.Id);
                    CsvHelper.InitializeOutputFileContents(outputFilePath, lotInfo.MakeMeasurementFileHeader());
                }

                Misc.MakeTemporaryFileWithStandardizedContents(inputFilePath, intermediateFilePath, lotInfo.Date, startMeasurementIndex);
                Misc.AppendOneFileToAnother(intermediateFilePath, outputFilePath);

                MessageBox.Show("Datele au fost importate in fisierul: " + Environment.NewLine + Environment.NewLine +
                    outputFilePath, "Conversie Terminata", MessageBoxButtons.OK, MessageBoxIcon.None);

                File.Delete(intermediateFilePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show("S-a detectat o eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK);
            }
            finally
            {
                uctlLotData.InitializeInputControls();
                uctlLotData.EnableInputControls();
                txtFileName.Text = "";
                dateTimePicker.Value = DateTime.Now;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
