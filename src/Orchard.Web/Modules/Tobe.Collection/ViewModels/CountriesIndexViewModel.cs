using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class CountriesIndexViewModel {
        public IEnumerable<Country> Countries { get; set; }
        public dynamic Pager { get; set; }
    }
}