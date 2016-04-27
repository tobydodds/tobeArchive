namespace Tobe.Collection.Models {
    public class AlbumAgentArtistRelation {
        public AlbumAgentArtistRelation() {}

        public AlbumAgentArtistRelation(AlbumPart album) {
            Album = album;
        }

        public AlbumPart Album { get; set; }
    }
}