using System;
using System.ComponentModel.DataAnnotations;

namespace Tobe.Collection.ViewModels {
    public class CreateAgentViewModel {
        [Required(ErrorMessage = "The agent's name is required")]
        public string Name { get; set; }
        public string NameSort { get; set; }
        public string FileUnder { get; set; }
    }
}