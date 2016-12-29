namespace Tobe.Collection.Models {
    public class AlbumGenreCategoryRelation {
        public AlbumGenreCategoryRelation() {}

        public AlbumGenreCategoryRelation(AlbumPart album) {
            Album = album;
        }

        public AlbumPart Album { get; set; }
    }
}