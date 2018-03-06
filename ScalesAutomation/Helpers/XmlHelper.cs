using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ScalesAutomation
{
    class XmlHelper
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<Product> Catalogue = new List<Product>();

        public void ReadCatalogue(String filePath)
        {
            try
            {
                var document = System.Xml.Linq.XDocument.Load(filePath);
                var root = document.Root;

                var elements = root.Elements("Product");
                foreach (var element in elements)
                {
                    var name = element.Elements("Name").First().Value;
                    var packageDetails = new Package();

                    packageDetails.Type = element.Elements("Package").First().Value;

                    var strNetWeight = element.Elements("NetWeight").First().Value.Replace(",", ".");
                    strNetWeight = Regex.Replace(strNetWeight, "Kg", "", RegexOptions.IgnoreCase);
                    Double.TryParse(strNetWeight, out packageDetails.NetWeight);

                    var strTare = element.Elements("PackageTare").First().Value.Replace(",", ".");
                    strTare = Regex.Replace(strTare, "Kg", "", RegexOptions.IgnoreCase);
                    Double.TryParse(strTare, out packageDetails.Tare);

                    packageDetails.TotalWeight = packageDetails.NetWeight + packageDetails.Tare;

                    AddToCatalogueOrUpdateIfExisting(name, packageDetails);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error reading the Product Catalogue: " + Environment.NewLine + ex.Message + Environment.NewLine);
            }
        }

        private void AddToCatalogueOrUpdateIfExisting(string name, Package packageDetails)
        {
            Product? temp = Catalogue.Find(x => x.Name == name);
            if (temp?.Name != null)
            {
                temp?.PackageDetails.Add(packageDetails);
            }
            else
            {
                var entry = new Product
                {
                    Name = name,
                    PackageDetails = new List<Package> { packageDetails }
                };

                Catalogue.Add(entry);
            }
        }
    }
}
