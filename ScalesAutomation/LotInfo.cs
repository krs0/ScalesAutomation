using log4net;
using System;
using System.IO;
using System.Reflection;

namespace ScalesAutomation
{
    public class LotInfo
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Lot;
        public string ProductName;
        public Package Package;
        public string Date;
        public int ZeroThreshold; //grams
        public bool TareIsSet; // This should be set by user when package Tare is setup on Scale
        public bool AppendToLot;
        public string Id => GetUniqueLotId();

        public LotInfo ReadLotInfoFromLog(string logFilePath)
        {
            var lotInfoFound = false;

            var lotInfo = new LotInfo();

            using (var file = new StreamReader(logFilePath))
            {
                var line = "";
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("### Lot Info ###"))
                    {
                        lotInfo.Lot = GetLotInfoValueString(file, "Lot");
                        lotInfo.ProductName = GetLotInfoValueString(file, "Product Name");
                        lotInfo.Package.Type = GetLotInfoValueString(file, "Package");
                        lotInfo.Package.NetWeight = GetLotInfoValue(file, "Net Weight");
                        lotInfo.Package.Tare = GetLotInfoValue(file, "Tare");
                        lotInfo.ZeroThreshold = GetLotInfoValue(file, "Zero Threshold");
                        lotInfo.Date = GetLotInfoValueString(file, "Date");

                        lotInfoFound = true;
                        break;
                    }
                }

                if (!lotInfoFound)
                    log.Error("No Lot Info found!");

            }

            return lotInfo;

            string GetLotInfoValueString(StreamReader file, string attributeName)
            {
                var line = file.ReadLine();
                if (line == null)
                    return "";

                attributeName += ": ";
                var splitLine = line.Split(new[] { attributeName }, StringSplitOptions.None);

                return splitLine[1];

            }

            int GetLotInfoValue(StreamReader file, string attributeName)
            {
                var rawValue = GetLotInfoValueString(file, attributeName);
                var valueInGrams = Misc.GetValueInGrams(rawValue);

                return valueInGrams;
            }
        }

        public string GetUniqueLotId()
        {
            var uniqueId = Lot + "_" + ProductName + "_" + Package.Type;
            //uniqueId = uniqueId.Replace(" ", ""); // No spaces in file names

            return uniqueId;
        }

        public string MakeMeasurementFileHeader()
        {
            var fileHeader = "#;Cantitatea Cantarita [Cc];Ora;" +
                             Lot + ";" +
                             ProductName + ";" +
                             Package.Type + ";" +
                             Package.NetWeight + ";" +
                             Package.Tare + ";" +
                             Date;

            return fileHeader;
        }

        public static LotInfo ReadMeasurementFileHeader(string outputFilePath)
        {
            var lotInfo = new LotInfo();

            using (var outputFile = new StreamReader(outputFilePath))
            {
                var line = outputFile.ReadLine() ?? throw new Exception("Fisierul de masuratori e gol!");
                {
                    if (!line.Contains("#;Cantitatea Cantarita [Cc];Ora"))
                        throw new Exception("Headerul fisierului de masuratori are un format necunoscut!");


                    var splitLine = line.Split(';');

                    lotInfo.Lot = splitLine[3];
                    lotInfo.ProductName = splitLine[4];
                    lotInfo.Package.Type = splitLine[5];

                    var netWeight = splitLine[6];
                    lotInfo.Package.NetWeight = Misc.GetValueInGrams(netWeight);

                    var tare = splitLine[7];
                    lotInfo.Package.Tare = Misc.GetValueInGrams(tare);

                    lotInfo.Date = splitLine[8];
                }
            }

            return lotInfo;
        }
    }
}