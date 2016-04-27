using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class RolesIndexViewModel {
        public IEnumerable<Agent> Roles { get; set; }
        public dynamic Pager { get; set; }
    }
}