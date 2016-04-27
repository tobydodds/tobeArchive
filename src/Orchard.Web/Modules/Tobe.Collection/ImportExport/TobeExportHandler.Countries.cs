using System;
using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Models;

namespace Tobe.Collection.ImportExport {
    public partial class TobeExportHandler {
        public XElement ExportCountries()
        {
            return new XElement("Countries", 
                _countryRepository.Table.OrderBy(x => x.Name)
                .Select(x => ToElement(x)));
        }

        private static XElement ToElement(Country country) {
            var countryElement = new XElement("Country");

            countryElement.Add(new XAttribute("Name", country.Name));
        
            return countryElement;
        }
    }
}