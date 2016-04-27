using System;
using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class CountryManager : ICountryManager {
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<ContentCountries> _contentCountriesRepository;
        private readonly IContentManager _contentManager;

        public CountryManager(
            IRepository<Country> countryRepository, 
            IRepository<ContentCountries> contentCountriesRepository, 
            IContentManager contentManager) {

            _countryRepository = countryRepository;
            _contentCountriesRepository = contentCountriesRepository;
            _contentManager = contentManager;
        }

        public IQueryable<Country> Query() {
            return _countryRepository.Table;
        }

        public void RemoveCountriesFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null) {
            var countriesQuery = _contentCountriesRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName);

            if (except != null) {
                countriesQuery = countriesQuery.Where(x => !except.Contains(x.Id));
            }

            foreach (var country in countriesQuery) {
                _contentCountriesRepository.Delete(country);
            }
        }

        public ContentCountries AddCountryToContent(ContentItem contentItem, string fieldName, Country country) {
            var contentCountries = new ContentCountries {
                ContentItem = contentItem.Record,
                FieldName = fieldName,
                Country = country
            };

            _contentCountriesRepository.Create(contentCountries);
            return contentCountries;
        }

        public IEnumerable<ContentItem> GetContentItemsByCountry(int countryId, VersionOptions versionOptions = null) {
            var contentItemIds = _contentCountriesRepository.Table.Where(x => x.Country.Id == countryId).Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<ContentItem> GetContentItemsByCountries(IEnumerable<int> countryIds, string contentType = null, VersionOptions versionOptions = null) {
            var query =
                from contentCountry in _contentCountriesRepository.Table
                where countryIds.Contains(contentCountry.Country.Id)
                select contentCountry;

            if (!String.IsNullOrWhiteSpace(contentType)) {
                query = 
                    from contentCountry in query 
                    where contentCountry.ContentItem.ContentType.Name == contentType 
                    select contentCountry;
            }

            var contentItemIds = query.Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<Country> GetCountriesByContent(int contentItemId, string fieldName) {
            return _contentCountriesRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName).Select(x => x.Country).Distinct();
        }
    }
}