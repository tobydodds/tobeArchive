using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Tobe.Collection.Models;

namespace Tobe.Collection.Fields {
    public class CountriesField : ContentField {
        internal LazyField<IEnumerable<Country>> _countriesField = new LazyField<IEnumerable<Country>>();

        public IEnumerable<Country> Countries {
            get { return _countriesField.Value; }
        } 
    }
}