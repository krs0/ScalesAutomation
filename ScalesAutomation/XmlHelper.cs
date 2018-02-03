using System;
using System.Collections.Generic;
using System.Linq;

namespace ScalesAutomation
{
    class XmlHelper
    {
        public struct Product
        {
            public String Name;
            public List<PackageDetails> PackageDetails;
        }

        public struct PackageDetails
        {
            public String Type;
            public String Tare;
            public String NetWeight;
        }

        public List<Product> catalogue = new List<Product>();

        public void Read(String filePath)
        {
            var document = System.Xml.Linq.XDocument.Load(filePath);
            var root = document.Root;

            var elements = root.Elements("Product");
            foreach (var element in elements)
            {
                var name = element.Elements("Name").First().Value;
                var packageDetails = new PackageDetails()
                {
                    Type = element.Elements("Package").First().Value,
                    Tare = element.Elements("PackageTare").First().Value,
                    NetWeight = element.Elements("NetWeight").First().Value
                };

                Product? temp = catalogue.Find(x => x.Name == name);
                if (temp?.Name != null)
                {
                    temp?.PackageDetails.Add(packageDetails);
                }
                else
                {
                    var entry = new Product
                    {
                        Name = name,
                        PackageDetails = new List<PackageDetails> { packageDetails }
                    };

                    catalogue.Add(entry);
                }
            }
        }
    }
}
