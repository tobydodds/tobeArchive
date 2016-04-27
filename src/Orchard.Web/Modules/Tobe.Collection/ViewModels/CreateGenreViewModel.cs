using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class CreateGenreViewModel {
        [Required(ErrorMessage = "The genre's name is required")]
        public string Name { get; set; }
    }
}