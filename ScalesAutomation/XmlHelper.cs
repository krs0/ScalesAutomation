using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScalesAutomation
{
    class XmlHelper
    {
        struct Catalog
        {
            String Name;
            String Package;
            String PackageTare;
            String NetWeight;

        }
        public Array catalog = new Array[10];
        void Read(String filePath)
        {
            var document = System.Xml.Linq.XDocument.Parse(filePath);
            var root = document.Root;

            var elements = root.Elements("Catalog");
            foreach (var element in elements)
            {
                String Name = element.Elements("Name").First().Value;
                String Package = element.Elements("Package").First().Value;
                String PackageTare = element.Elements("PackageTare").First().Value;
                String NetWeight = element.Elements("NetWeight").First().Value;
            }
        }
    }
}
