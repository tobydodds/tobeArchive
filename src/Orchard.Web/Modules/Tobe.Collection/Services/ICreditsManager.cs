using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface ICreditsManager : IDependency {
        IEnumerable<Agent> GetAgents(IEnumerable<int> ids = null);
        IEnumerable<Role> GetRoles(IEnumerable<int> ids = null);
        IEnumerable<CreditsContainerPart> GetCreditsContainersByAgent(int agentId, VersionOptions versionOptions = null);
        IEnumerable<CreditsContainerPart> GetCreditsContainersByRole(int roleId, VersionOptions versionOptions = null);
        IQueryable<Credit> GetCreditsByAgent(int agentId);
        IQueryable<Credit> GetCreditsByRole(int roleId);
        void RemoveCreditByAgent(CreditsContainerPart container, int agentId);
        Credit AddCredit(CreditsContainerPart container, Agent agent, Role role);
        IEnumerable<Credit> GetCredits(int containerId);
        void ClearCredits(int containerId, IEnumerable<int> except = null);
        void DeleteAgent(Agent agent);
        void DeleteRole(Role role);
        Agent GetAgentByName(string name);
    }
}