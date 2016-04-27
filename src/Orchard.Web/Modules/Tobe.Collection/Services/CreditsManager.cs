using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class CreditsManager : ICreditsManager {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Credit> _creditRepository;
        private readonly IContentManager _contentManager;

        public CreditsManager(
            IRepository<Credit> creditRepository, 
            IContentManager contentManager, 
            IRepository<Agent> agentRepository,
            IRepository<Role> roleRepository) {

            _agentRepository = agentRepository;
            _roleRepository = roleRepository;
            _creditRepository = creditRepository;
            _contentManager = contentManager;
        }

        public IEnumerable<Agent> GetAgents(IEnumerable<int> ids = null) {
            var query = _agentRepository.Table;
            if (ids != null) query = query.Where(x => ids.ToArray().Contains(x.Id));
            return query.ToList();
        }

        public IEnumerable<Role> GetRoles(IEnumerable<int> ids = null) {
            var query = _roleRepository.Table;
            if (ids != null) query = query.Where(x => ids.ToArray().Contains(x.Id));
            return query.ToList();
        }

        public IEnumerable<CreditsContainerPart> GetCreditsContainersByAgent(int agentId, VersionOptions versionOptions = null) {
            var credits = GetCreditsByAgent(agentId).Select(x => x.ContainerId);
            return _contentManager.GetMany<CreditsContainerPart>(credits, versionOptions ?? VersionOptions.Published, QueryHints.Empty).ToList();
        }

        public IEnumerable<CreditsContainerPart> GetCreditsContainersByRole(int roleId, VersionOptions versionOptions = null) {
            var credits = GetCreditsByRole(roleId).Select(x => x.ContainerId);
            return _contentManager.GetMany<CreditsContainerPart>(credits, versionOptions ?? VersionOptions.Published, QueryHints.Empty).ToList();
        }

        public IQueryable<Credit> GetCreditsByAgent(int agentId) {
            return _creditRepository.Fetch(x => x.Agent.Id == agentId).AsQueryable();
        }

        public IQueryable<Credit> GetCreditsByRole(int roleId) {
            return _creditRepository.Fetch(x => x.Role.Id == roleId).AsQueryable();
        }

        public void RemoveCreditByAgent(CreditsContainerPart container, int agentId) {
            var credit = container.Credits.FirstOrDefault(x => x.Agent.Id == agentId);

            if (credit == null)
                return;

            _creditRepository.Delete(credit);
        }

        public Credit AddCredit(CreditsContainerPart container, Agent agent, Role role) {
            var credit = new Credit {
                ContainerId = container.Id,
                Agent = agent,
                Role = role
            };
            _creditRepository.Create(credit);
            return credit;
        }

        public IEnumerable<Credit> GetCredits(int containerId) {
            var query = from credit in _creditRepository.Fetch(x => x.ContainerId == containerId)
                        select new {
                            record = credit, 
                            agent = credit.Agent  // Eager loading.
                        };
            return query.Select(x => x.record).ToList();
        }

        public void ClearCredits(int containerId, IEnumerable<int> except = null) {
            var query = GetCredits(containerId);

            if (except != null) {
                query = query.Where(x => !except.Contains(x.Id)).ToList();
            }

            foreach (var credit in query) {
                _creditRepository.Delete(credit);
            }
        }

        public void DeleteAgent(Agent agent) {
            _agentRepository.Delete(agent);
        }

        public void DeleteRole(Role role) {
            _roleRepository.Delete(role);
        }

        public Agent GetAgentByName(string name) {
            return _agentRepository.Get(x => x.Name == name);
        }
    }
}