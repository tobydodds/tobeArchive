using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Helpers;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Drivers {
    public class CategoriesContainerPartDriver : ContentPartDriver<CategoriesContainerPart> {
        private readonly ICategoriesManager _categoriesManager;

        public CategoriesContainerPartDriver(ICategoriesManager categoriesManager) {
            _categoriesManager = categoriesManager;
        }

        protected override DriverResult Display(CategoriesContainerPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Categories", () => shapeHelper.Parts_Categories());
        }

        protected override DriverResult Editor(CategoriesContainerPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(CategoriesContainerPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_Categories_Edit", () => {
                var viewModel = new CategoriesContainerViewModel() {
                    AllGenres = _categoriesManager.GetGenres().OrderBy(x => x.Name).ToList(),
                };

                viewModel.Categories = part.Categories.Select(x => new CategoryViewModel {
                    Id = x.Id,
                    GenreId = x.Genre.Id,
                    GenreName = x.Genre.Name
                }).ToList();

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AllGenres" })) {
                        UpdateCategories(viewModel, part);
                    }
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/Categories", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override void Exporting(CategoriesContainerPart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            var categories = part.Categories;

            foreach (var category in categories) {
                var categoryElement = new XElement("Category");

                categoryElement.AddAttribute("GenreName", category.Genre != null ? category.Genre.Name : default(string));

                partElement.Add(categoryElement);
            }
        }

        protected override void Importing(CategoriesContainerPart part, ImportContentContext context) {
            var partElement = context.Data.Element(part.PartDefinition.Name);

            if (partElement == null)
                return;

            var genresDictionary = _categoriesManager.GetGenres().Distinct(new DistinctGenreComparer()).ToDictionary(x => x.Name);

            foreach (var categoryElement in partElement.Elements("Category")) {
                var genre = categoryElement.ImportAttribute<string, Genre>("GenreName", x => genresDictionary.ContainsKey(x) ? genresDictionary[x] : default(Genre));
                _categoriesManager.AddCategory(part, genre);
            }
        }

        private void UpdateCategories(CategoriesContainerViewModel viewModel, CategoriesContainerPart part) {
            var genres = viewModel.Categories != null ? _categoriesManager.GetGenres(viewModel.Categories.Select(x => x.GenreId)) : Enumerable.Empty<Genre>();

            if (viewModel.Categories == null)
                return;

            foreach (var categoryViewModel in viewModel.Categories) {
                if (categoryViewModel.Removed) {
                    var category = categoryViewModel.Id > 0 ? _categoriesManager.GetCategory(categoryViewModel.Id) : default(Category);

                    _categoriesManager.RemoveCategory(category);
                }
                else {
                    if (categoryViewModel.Id == 0) {
                        var genre = genres.Single(x => x.Id == categoryViewModel.GenreId);
                        _categoriesManager.AddCategory(part, genre);
                    }
                }
            }
        }
    }
}