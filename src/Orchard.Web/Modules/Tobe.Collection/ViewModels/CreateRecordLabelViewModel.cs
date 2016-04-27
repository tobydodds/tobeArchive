using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class CreateRecordLabelViewModel {
        [Required(ErrorMessage = "The record label's name is required")]
        public string Name { get; set; }
    }
}