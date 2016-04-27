using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class CreateCountryViewModel {
        [Required(ErrorMessage = "The country's name is required")]
        public string Name { get; set; }
    }
}