using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Handlers {
    public class CategoriesContainerPartHandler : ContentHandler {
        private readonly ICategoriesManager _categoriesManager;

        public CategoriesContainerPartHandler(ICategoriesManager categoriesManager) {
            _categoriesManager = categoriesManager;
            OnActivated<CategoriesContainerPart>(SetupFields);
            OnRemoved<CategoriesContainerPart>(DeleteCategories);
        }

        private void DeleteCategories(RemoveContentContext context, CategoriesContainerPart part) {
            _categoriesManager.ClearCategories(part.Id);
        }

        private void SetupFields(ActivatedContentContext context, CategoriesContainerPart part) {
            part.CategoriesField.Loader(x => _categoriesManager.GetCategories(part.Id));
        }
    }
}