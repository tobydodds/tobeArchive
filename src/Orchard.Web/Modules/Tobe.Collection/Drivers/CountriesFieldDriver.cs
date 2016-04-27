using System.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Tobe.Collection.Drivers {
    public class CountriesFieldDriver : ContentFieldDriver<CountriesField> {
        private readonly ICountryManager _countryManager;

        public CountriesFieldDriver(ICountryManager countryManager) {
            _countryManager = countryManager;
        }

        protected override DriverResult Display(ContentPart part, CountriesField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Countries", GetDifferentiator(field), () => shapeHelper.Fields_Countries(Countries: _countryManager.GetCountriesByContent(part.Id, field.Name).ToArray()));
        }

        protected override DriverResult Editor(ContentPart part, CountriesField field, dynamic shapeHelper) {
            return Editor(part, field, null, shapeHelper);
        }

        protected override DriverResult Editor(ContentPart part, CountriesField field, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Fields_Countries_Edit", GetDifferentiator(field), () => {
                var prefix = GetPrefix(field, part);
                var model = new CountriesViewModel {
                    ContentField = field,
                    AllCountries = _countryManager.Query().ToArray()
                };

                if (updater != null) {
                    if (updater.TryUpdateModel(model, prefix, null, null)) {
                        UpdateCountries(part, field, model);
                    }
                }

                var countries = _countryManager.GetCountriesByContent(part.Id, field.Name);
                model.Countries = countries.Select(x => new CountryViewModel {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
                return shapeHelper.EditorTemplate(TemplateName: "Fields/Countries", Model: model, Prefix: prefix);
            });
        }

        private void UpdateCountries(ContentPart part, CountriesField field, CountriesViewModel model) {
            var postedCountryIds = model.Countries != null ? model.Countries.Select(x => x.Id).ToArray() : Enumerable.Empty<int>();
            var postedCountries = _countryManager.Query().Where(x => postedCountryIds.Contains(x.Id)).ToArray();
            var countries = _countryManager.GetCountriesByContent(part.Id, field.Name);

            _countryManager.RemoveCountriesFromContent(part.Id, field.Name, postedCountryIds);

            foreach (var postedCountry in
                    from postedCountry in postedCountries
                    let country = countries.FirstOrDefault(x => x.Id == postedCountry.Id)
                    where country == null
                    select postedCountry) {
                _countryManager.AddCountryToContent(part.ContentItem, field.Name, postedCountry);
            }
        }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field) {
            return field.Name;
        }
    }
}