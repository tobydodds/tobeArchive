using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface ICountryManager : IDependency {
        IQueryable<Country> Query();
        void RemoveCountriesFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null);
        ContentCountries AddCountryToContent(ContentItem contentItem, string fieldName, Country country);
        IEnumerable<ContentItem> GetContentItemsByCountry(int countryId, VersionOptions versionOptions = null);
        IEnumerable<ContentItem> GetContentItemsByCountries(IEnumerable<int> countryIds, string contentType = null, VersionOptions versionOptions = null);
        IEnumerable<Country> GetCountriesByContent(int contentItemId, string fieldName);
    }
}