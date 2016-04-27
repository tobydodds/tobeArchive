using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class GenresIndexViewModel {
        public IEnumerable<Genre> Genres { get; set; }
        public dynamic Pager { get; set; }
    }
}