using System;
using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class GenreManager : IGenreManager {
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<ContentGenres> _contentGenresRepository;
        private readonly IContentManager _contentManager;

        public GenreManager(
            IRepository<Genre> genreRepository, 
            IRepository<ContentGenres> contentGenresRepository, 
            IContentManager contentManager) {

            _genreRepository = genreRepository;
            _contentGenresRepository = contentGenresRepository;
            _contentManager = contentManager;
        }

        public IQueryable<Genre> Query() {
            return _genreRepository.Table;
        }

        public void RemoveGenresFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null) {
            var genresQuery = _contentGenresRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName);

            if (except != null) {
                genresQuery = genresQuery.Where(x => !except.Contains(x.Id));
            }

            foreach (var genre in genresQuery) {
                _contentGenresRepository.Delete(genre);
            }
        }

        public ContentGenres AddGenreToContent(ContentItem contentItem, string fieldName, Genre genre) {
            var contentGenres = new ContentGenres {
                ContentItem = contentItem.Record,
                FieldName = fieldName,
                Genre = genre
            };

            _contentGenresRepository.Create(contentGenres);
            return contentGenres;
        }

        public IEnumerable<ContentItem> GetContentItemsByGenre(int genreId, VersionOptions versionOptions = null) {
            var contentItemIds = _contentGenresRepository.Table.Where(x => x.Genre.Id == genreId).Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<ContentItem> GetContentItemsByGenres(IEnumerable<int> genreIds, string contentType = null, VersionOptions versionOptions = null) {
            var query = 
                from contentGenre in _contentGenresRepository.Table
                where genreIds.Contains(contentGenre.Genre.Id) 
                select contentGenre;

            if (!String.IsNullOrWhiteSpace(contentType)) {
                query =
                    from contentGenre in query 
                    where contentGenre.ContentItem.ContentType.Name == contentType 
                    select contentGenre;
            }

            var contentItemIds = query.Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<Genre> GetGenresByContent(int contentItemId, string fieldName) {
            return _contentGenresRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName).Select(x => x.Genre).Distinct();
        }
    }
}