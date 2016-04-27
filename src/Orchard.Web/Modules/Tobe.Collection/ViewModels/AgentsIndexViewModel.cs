using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class AgentsIndexViewModel {
        public IEnumerable<Agent> Agents { get; set; }
        public dynamic Pager { get; set; }
    }
}