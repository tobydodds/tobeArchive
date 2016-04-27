using System;
using System.Linq;
using System.Xml.Linq;
using Orchard.ImportExport.Models;
using Tobe.Collection.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.ImportExport.Services;

namespace Tobe.Collection.ImportExport {
    [OrchardFeature("Tobe.Collection.ImportExport")]
    public partial class TobeExportHandler : IExportEventHandler {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IRepository<Country> _countryRepository;

        public TobeExportHandler(IRepository<Agent> agentRepository,
            IRepository<Country> countryRepository) {
            _agentRepository = agentRepository;
            _countryRepository = countryRepository;
        }

        public void Exporting(ExportContext context) {
        }

        public void Exported(ExportContext context) {
            if (!(context.ExportOptions.CustomSteps).Contains("TobeGlossary")) {
                return;
            }

            context.Document.Element("Orchard").Add(
                new XElement("TobeGlossary",
                    ExportAgents(),
                    ExportCountries()));
        }

        private static XAttribute AddAttribute(XElement element, string attributeName, object value) {
            if (value == null)
                return null;

            var attribute = new XAttribute(attributeName, value);
            element.Add(attribute);
            return attribute;
        }
    }
}