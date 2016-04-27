using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Tobe.Collection.ViewModels {
    public class EditGenreViewModel {
        public int Id { get; set; }

        [Required(ErrorMessage = "The genre's name is required")]
        public string Name { get; set; }

        public IList<ContentItem> RelatedContent { get; set; }
    }
}