using System.Collections.Generic;
using System.Data.SqlClient;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class CategoriesContainerViewModel {
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<Genre> AllGenres { get; set; }
    }
}