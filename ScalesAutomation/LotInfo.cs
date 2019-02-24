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
        public double ZeroThreshold;
        public bool AppendToLot;

        // log file will contain 2 normalizedMeasurements at the top in this order
        // 2019-01-07 15:59:26,051 INFO Net Weight: 10000
        // 2019-01-07 15:59:26,051 INFO Zero Threshold: 7500
        public LotInfo ReadLotInfo(string logFilePath)
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
                        lotInfo.Package.NetWeight = GetLotInfoValueDouble(file, "Net Weight");
                        lotInfo.Package.Tare = GetLotInfoValueDouble(file, "Tare");
                        lotInfo.ZeroThreshold = GetLotInfoValueDouble(file, "Zero Threshold");
                        lotInfo.Date = GetLotInfoValueString(file, "Date");

                        lotInfoFound = true;
                        break;
                    }
                }

                if (!lotInfoFound)
                    log.Error("No Lot Info found!");

                file.Close();
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

            double GetLotInfoValueDouble(StreamReader file, string attributeName)
            {
                double value;
                var rawValue = GetLotInfoValueString(file, attributeName);

                if (rawValue.Contains("Kg"))
                {
                    Misc.RemoveTrailingKg(ref rawValue);
                    double.TryParse(rawValue, out value);
                    value = value * 1000;
                }
                else
                {
                    double.TryParse(rawValue, out value);
                }

                return value;
            }
        }
    }
}