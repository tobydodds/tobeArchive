using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Tobe.Collection.Models {
    public class CategoriesContainerPart : ContentPart {
        internal LazyField<IEnumerable<Category>> CategoriesField = new LazyField<IEnumerable<Category>>();

        public IEnumerable<Category> Categories {
            get { return CategoriesField.Value; }
        }
    }
}