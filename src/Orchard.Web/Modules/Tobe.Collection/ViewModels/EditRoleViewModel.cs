using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class EditRoleViewModel {
        public int Id { get; set; }

        [Required(ErrorMessage = "The role's name is required")]
        public string Name { get; set; }
    }
}