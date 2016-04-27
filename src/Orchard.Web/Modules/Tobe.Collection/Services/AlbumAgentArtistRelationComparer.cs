using System;
using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Services {
    public class AlbumAgentArtistRelationComparer : IEqualityComparer<AlbumAgentArtistRelation> {
        public bool Equals(AlbumAgentArtistRelation x, AlbumAgentArtistRelation y) {
            return x.Album.Id == y.Album.Id;
        }

        public int GetHashCode(AlbumAgentArtistRelation obj) {
            return obj.Album.Id.GetHashCode();
        }
    }
}