using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Helpers {
    public class DistinctGenreComparer : IEqualityComparer<Genre> {
        public bool Equals(Genre x, Genre y) {
            return x.Name == y.Name;
        }

        public int GetHashCode(Genre obj) {
            return obj.Name.GetHashCode();
        }
    }
}