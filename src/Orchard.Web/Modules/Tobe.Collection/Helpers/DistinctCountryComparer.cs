using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Helpers {
    public class DistinctCountryComparer : IEqualityComparer<Country> {
        public bool Equals(Country x, Country y) {
            return x.Name == y.Name;
        }

        public int GetHashCode(Country obj) {
            return obj.Name.GetHashCode();
        }
    }
}