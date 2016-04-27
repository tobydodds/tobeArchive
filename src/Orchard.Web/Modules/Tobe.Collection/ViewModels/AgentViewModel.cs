using System;
using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class AgentViewModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameSort { get; set; }
        public string FileUnder { get; set; }
        public IList<AlbumAgentArtistRelation> RelatedAlbumArtists { get; set; }
        public IList<AlbumAgentCreditRelation> RelatedAlbumCredits { get; set; }
        public IList<CreditsContainerPart> CreditContainers { get; set; }
    }
}