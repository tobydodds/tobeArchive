using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;

namespace Tobe.Collection.ImportExport {
    public partial class TobeImportHandler {
        
        private void ImportCountries(XElement root) {
            var countriesElement = root.Element("Countries");

            if (countriesElement != null) {
                var countries = _countryRepository.Table.ToArray();

                foreach (var countryElement in countriesElement.Elements("Country")) {
                    var countryName = countryElement.Attr<string>("Name");
                    var country = countries.FirstOrDefault(x => x.Name == countryName);

                    if (country == null) {
                        country = new Country
                        {
                            Name = countryName
                        };
                        _countryRepository.Create(country);
                    }
                }
            }
        }
    }
}
