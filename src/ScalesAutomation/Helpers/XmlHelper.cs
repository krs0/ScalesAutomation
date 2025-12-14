using log4net;

namespace ScalesAutomation;

class XmlHelper
{
    private static readonly ILog log = LogManager.GetLogger("generalLog");

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

                var strNetWeight = element.Elements("NetWeight").First().Value;
                packageDetails.NetWeight = Misc.GetValueInGrams(strNetWeight);

                var strTare = element.Elements("PackageTare").First().Value;
                packageDetails.Tare = Misc.GetValueInGrams(strTare);

                packageDetails.TotalWeight = packageDetails.NetWeight + packageDetails.Tare;

                AddToCatalogueCollection(name, packageDetails);
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error reading the Product Catalogue:{Environment.NewLine}{ex.Message}");
            //throw new Exception($"Eroare la citirea Catalogului de Produse: {filePath}"); // <- this causes error in designer
        }
    }

    /// <summary>
    /// Adds product data to catalogue collection. Updates if product already existing.
    /// </summary>
    private void AddToCatalogueCollection(string name, Package packageDetails)
    {
        Product? searchedProduct = Catalogue.Find(x => x.Name == name);
        if (searchedProduct?.Name != null)
            searchedProduct?.PackageDetails.Add(packageDetails);
        else
        {
            var product = new Product
            {
                Name = name,
                PackageDetails = new List<Package> { packageDetails }
            };

            Catalogue.Add(product);
        }
    }
}
