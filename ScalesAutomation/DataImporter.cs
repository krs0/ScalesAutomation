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
            var inputFilePath = txtFileName.Text;
            var outputFolderPath = Settings.Default.CSVServerFolderPath;
            var lotOutputFilePath = "";
            var intermediateFilePath = Path.GetDirectoryName(inputFilePath) + "\\temp%.bak";

            if (!uctlLotData.CheckInputControls())
                return;

            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Nu s-a specificat un fisier cu date de intrare!", "Date Incorecte", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lotInfo = uctlLotData.LotInfo;
            lotInfo.Date = dateTimePicker.Value.ToString("yyyy-MM-dd");

            if (uctlLotData.LotInfo.AppendToLot)
            {
                lotOutputFilePath = CsvHelper.GetExistingLotOutputFileName(lotInfo.Lot, outputFolderPath);
                var lastLine = File.ReadLines(lotOutputFilePath).Last();
                var splitLine = lastLine.Split(';');
                var lastMeasurementIndex = splitLine[0];
                Misc.MakeTemporaryFileWithStandardizedContents(inputFilePath, intermediateFilePath, lotInfo.Date, Int32.Parse(lastMeasurementIndex));

                Misc.AppendOneFileToAnother(intermediateFilePath, lotOutputFilePath);
            }
            else
            {
                var productInfo = CsvHelper.MakeProductInfo(lotInfo);
                lotOutputFilePath = CsvHelper.MakeOutputFilePath(outputFolderPath, DateTime.Now, productInfo);

                CsvHelper.InitializeOutputFile(lotOutputFilePath, CsvHelper.CreateMeasurementFileHeader(lotInfo));
                Misc.MakeTemporaryFileWithStandardizedContents(inputFilePath, intermediateFilePath, lotInfo.Date, 0);

                Misc.AppendOneFileToAnother(intermediateFilePath, lotOutputFilePath);
            }

            MessageBox.Show("Datele au fost importate in fisierul: " + Environment.NewLine +  Environment.NewLine + lotOutputFilePath, "Conversie Completa", MessageBoxButtons.OK, MessageBoxIcon.None);

            File.Delete(intermediateFilePath);
            uctlLotData.InitializeInputControls();
            uctlLotData.EnableInputControls();
            txtFileName.Text = "";
            dateTimePicker.Value = DateTime.Now;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
