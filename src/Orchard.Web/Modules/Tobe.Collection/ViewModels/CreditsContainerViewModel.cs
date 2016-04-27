using System.Collections.Generic;
using System.Data.SqlClient;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class CreditsContainerViewModel {
        public IEnumerable<CreditViewModel> Credits { get; set; }
        public IEnumerable<Role> AllRoles { get; set; }
    }
}