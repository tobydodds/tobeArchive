using System.Collections.Generic;
using Tobe.Collection.Fields;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class CountriesViewModel {
        public CountriesViewModel() {
            Countries = new List<CountryViewModel>();
        }
        public IList<CountryViewModel> Countries { get; set; }
        public IList<Country> AllCountries { get; set; }
        public CountriesField ContentField { get; set; }
    }
}