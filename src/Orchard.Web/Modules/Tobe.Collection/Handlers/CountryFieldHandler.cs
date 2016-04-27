using System;
using System.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Resources;

namespace Tobe.Collection.Handlers {
    public class CountryFieldHandler : ContentHandler {
        private readonly ICountryManager _countryManager;
        private readonly IResourceManager _resourceManager;

        public CountryFieldHandler(ICountryManager countryManager, IResourceManager resourceManager) {
            _countryManager = countryManager;
            _resourceManager = resourceManager;
        }

        protected override void BuildDisplayShape(BuildDisplayContext context) {
            if (context.DisplayType != "Detail")
                return;

            var countriesFields = context.ContentItem.Parts.SelectMany(part => part.Fields.Where(field => field.FieldDefinition.Name == typeof (CountriesField).Name)).Cast<CountriesField>();
            var countries = countriesFields.SelectMany(x => _countryManager.GetCountriesByContent(context.ContentItem.Id, x.Name)).Select(x => x.Name).Distinct().ToArray();

            if (!countries.Any())
                return;

            var existingMeta = _resourceManager.GetRegisteredMetas().FirstOrDefault(x => x.Name == "Country");

            if (existingMeta != null) {
                var countryNames = existingMeta.Content.Split('|').ToList();
                countryNames.AddRange(countries);
                countries = countryNames.Distinct().ToArray();
            }

            _resourceManager.SetMeta(new MetaEntry {
                Name = "Country",
                Content = String.Join("|", countries)
            });
        }

        protected override void Initialized(InitializingContentContext context) {
            var countriesFields = context.ContentItem.Parts.SelectMany(x => x.Fields).Where(x => x.FieldDefinition.Name == typeof (CountriesField).Name).Select(x => (CountriesField)x);

            foreach (var field in countriesFields) {
                var closureField = field;
                field._countriesField.Loader(() => _countryManager.GetCountriesByContent(context.ContentItem.Id, closureField.Name).ToArray());
            }
        }
    }
}