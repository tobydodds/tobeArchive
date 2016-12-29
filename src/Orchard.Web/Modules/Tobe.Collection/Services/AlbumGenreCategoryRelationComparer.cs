using System;
using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Services {
    public class AlbumGenreCategoryRelationComparer : IEqualityComparer<AlbumGenreCategoryRelation> {
        public bool Equals(AlbumGenreCategoryRelation x, AlbumGenreCategoryRelation y) {
            return x.Album.Id == y.Album.Id;
        }

        public int GetHashCode(AlbumGenreCategoryRelation obj) {
            return obj.Album.Id.GetHashCode();
        }
    }
}