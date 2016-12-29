using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface ICategoriesManager : IDependency {
        IEnumerable<Genre> GetGenres(IEnumerable<int> ids = null);
        IEnumerable<CategoriesContainerPart> GetCategoriesContainersByGenre(int genreId, VersionOptions versionOptions = null);
        IQueryable<Category> GetCategoriesByGenre(int genreId);
        void RemoveCategory(Category category);
        void RemoveCategoryByGenre(CategoriesContainerPart container, int genreId);
        Category AddCategory(CategoriesContainerPart container, Genre genre);
        Category GetCategory(int id);
        IEnumerable<Category> GetCategories(int containerId);
        void ClearCategories(int containerId, IEnumerable<int> except = null);
        void DeleteGenre(Genre genre);
        Genre GetGenreByName(string name);
    }
}