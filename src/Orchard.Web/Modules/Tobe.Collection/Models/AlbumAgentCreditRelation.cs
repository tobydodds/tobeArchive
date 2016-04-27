namespace Tobe.Collection.Models {
    public class AlbumAgentCreditRelation {
        public AlbumAgentCreditRelation() {}

        public AlbumAgentCreditRelation(AlbumPart album, string role) {
            Album = album;
            Role = role;
        }

        public AlbumPart Album { get; set; }
        public string Role { get; set; }
    }
}