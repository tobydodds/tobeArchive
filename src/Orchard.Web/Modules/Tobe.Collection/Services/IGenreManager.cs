using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface IGenreManager : IDependency {
        IQueryable<Genre> Query();
        void RemoveGenresFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null);
        ContentGenres AddGenreToContent(ContentItem contentItem, string fieldName, Genre genre);
        IEnumerable<ContentItem> GetContentItemsByGenre(int genreId, VersionOptions versionOptions = null);
        IEnumerable<ContentItem> GetContentItemsByGenres(IEnumerable<int> genreIds, string contentType = null, VersionOptions versionOptions = null);
        IEnumerable<Genre> GetGenresByContent(int contentItemId, string fieldName);
    }
}