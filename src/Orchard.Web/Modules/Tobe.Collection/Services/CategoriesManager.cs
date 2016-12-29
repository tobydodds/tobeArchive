using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class CategoriesManager : ICategoriesManager {
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IContentManager _contentManager;

        public CategoriesManager(
            IRepository<Category> categoryRepository, 
            IContentManager contentManager, 
            IRepository<Genre> genreRepository) {

            _genreRepository = genreRepository;
            _categoryRepository = categoryRepository;
            _contentManager = contentManager;
        }

        public IEnumerable<Genre> GetGenres(IEnumerable<int> ids = null) {
            var query = _genreRepository.Table;
            if (ids != null) query = query.Where(x => ids.ToArray().Contains(x.Id));
            return query.ToList();
        }

        public IEnumerable<CategoriesContainerPart> GetCategoriesContainersByGenre(int genreId, VersionOptions versionOptions = null) {
            var categories = GetCategoriesByGenre(genreId).Select(x => x.ContainerId);
            return _contentManager.GetMany<CategoriesContainerPart>(categories, versionOptions ?? VersionOptions.Published, QueryHints.Empty).ToList();
        }

        public IQueryable<Category> GetCategoriesByGenre(int genreId) {
            return _categoryRepository.Fetch(x => x.Genre.Id == genreId).AsQueryable();
        }

        public void RemoveCategory(Category category) {
            if (category == null)
                return;

            _categoryRepository.Delete(category);
        }

        public void RemoveCategoryByGenre(CategoriesContainerPart container, int genreId) {
            var category = container.Categories.FirstOrDefault(x => x.Genre.Id == genreId);

            RemoveCategory(category);
        }

        public Category AddCategory(CategoriesContainerPart container, Genre genre) {
            var category = new Category {
                ContainerId = container.Id,
                Genre = genre
            };
            _categoryRepository.Create(category);
            return category;
        }

        public Category GetCategory(int id) {
            return _categoryRepository.Get(id);
        }

        public IEnumerable<Category> GetCategories(int containerId) {
            var query = from category in _categoryRepository.Fetch(x => x.ContainerId == containerId)
                        select new {
                            record = category, 
                            genre = category.Genre  // Eager loading.
                        };
            return query.Select(x => x.record).ToList();
        }

        public void ClearCategories(int containerId, IEnumerable<int> except = null) {
            var query = GetCategories(containerId);

            if (except != null) {
                query = query.Where(x => !except.Contains(x.Id)).ToList();
            }

            foreach (var category in query) {
                _categoryRepository.Delete(category);
            }
        }

        public void DeleteGenre(Genre genre) {
            _genreRepository.Delete(genre);
        }

        public Genre GetGenreByName(string name) {
            return _genreRepository.Get(x => x.Name == name);
        }
    }
}