using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class EditAgentViewModel {
        public int Id { get; set; }

        [Required(ErrorMessage = "The agent's name is required")]
        public string Name { get; set; }
        public string NameSort { get; set; }
        public string FileUnder { get; set; }

        public IList<AlbumAgentArtistRelation> RelatedAlbumArtists { get; set; }
        public IList<AlbumAgentCreditRelation> RelatedAlbumCredits { get; set; }
        public IList<CreditsContainerPart> RelatedContent { get; set; }
    }
}