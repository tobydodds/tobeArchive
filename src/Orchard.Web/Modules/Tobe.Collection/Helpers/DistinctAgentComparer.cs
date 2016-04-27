using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.Helpers {
    public class DistinctAgentComparer : IEqualityComparer<Agent> {
        public bool Equals(Agent x, Agent y) {
            return x.Name == y.Name;
        }

        public int GetHashCode(Agent obj) {
            return obj.Name.GetHashCode();
        }
    }
}