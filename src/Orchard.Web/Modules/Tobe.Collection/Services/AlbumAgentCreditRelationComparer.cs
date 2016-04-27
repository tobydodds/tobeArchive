using System;
using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Services {
    public class AlbumAgentCreditRelationComparer : IEqualityComparer<AlbumAgentCreditRelation> {
        public bool Equals(AlbumAgentCreditRelation x, AlbumAgentCreditRelation y) {
            return x.Album.Id == y.Album.Id && String.Equals(x.Role, y.Role, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(AlbumAgentCreditRelation obj) {
            return obj.Album.Id.GetHashCode() | obj.Role.GetHashCode();
        }
    }
}