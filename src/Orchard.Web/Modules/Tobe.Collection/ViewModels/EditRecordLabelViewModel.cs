using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Tobe.Collection.ViewModels {
    public class EditRecordLabelViewModel {
        public int Id { get; set; }

        [Required(ErrorMessage = "The record label's name is required")]
        public string Name { get; set; }

        public IList<ContentItem> RelatedContent { get; set; }
    }
}