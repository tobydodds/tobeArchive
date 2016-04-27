using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class GenresViewModel {
        public GenresViewModel() {
            Genres = new List<GenreViewModel>();
        }
        public IList<GenreViewModel> Genres { get; set; }
        public Genre[] AllGenres { get; set; }
        public GenresField ContentField { get; set; }
    }
}