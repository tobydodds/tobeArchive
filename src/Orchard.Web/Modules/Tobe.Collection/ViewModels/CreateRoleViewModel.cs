using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class CreateRoleViewModel {
        [Required(ErrorMessage = "The role's name is required")]
        public string Name { get; set; }
    }
}