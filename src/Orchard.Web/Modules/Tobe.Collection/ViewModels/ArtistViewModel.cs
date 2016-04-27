namespace Tobe.Collection.ViewModels {
    public class ArtistViewModel {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public int? DelimiterId { get; set; }
        public string DelimiterName { get; set; }
        public bool Removed { get; set; }
    }
}