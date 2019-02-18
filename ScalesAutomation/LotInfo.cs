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
                        lotInfo.Package.NetWeight = GetLotInfoValueInt(file, "Net Weight");
                        lotInfo.Package.Tare = GetLotInfoValueInt(file, "Tare");
                        lotInfo.ZeroThreshold = GetLotInfoValueInt(file, "Zero Threshold");
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

            int GetLotInfoValueInt(StreamReader file, string attributeName)
            {
                int.TryParse(GetLotInfoValueString(file, attributeName), out var value);

                return value;
            }
        }
    }
}