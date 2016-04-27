using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class ArtistsManager : IArtistsManager {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IRepository<Delimiter> _delimiterRepository;
        private readonly IRepository<Artist> _artistRepository;
        private readonly IContentManager _contentManager;

        public ArtistsManager(
            IRepository<Artist> artistRepository, 
            IContentManager contentManager, 
            IRepository<Agent> agentRepository,
            IRepository<Delimiter> delimiterRepository) {

            _agentRepository = agentRepository;
            _delimiterRepository = delimiterRepository;
            _artistRepository = artistRepository;
            _contentManager = contentManager;
        }

        public IEnumerable<Agent> GetAgents(IEnumerable<int> ids = null) {
            var query = _agentRepository.Table;
            if (ids != null) query = query.Where(x => ids.ToArray().Contains(x.Id));
            return query.ToList();
        }

        public IEnumerable<Delimiter> GetDelimiters(IEnumerable<int> ids = null) {
            var query = _delimiterRepository.Table;
            if (ids != null) query = query.Where(x => ids.ToArray().Contains(x.Id));
            return query.ToList();
        }

        public IEnumerable<ArtistsContainerPart> GetArtistsContainersByAgent(int agentId, VersionOptions versionOptions = null) {
            var artists = GetArtistsByAgent(agentId).Select(x => x.ContainerId);
            return _contentManager.GetMany<ArtistsContainerPart>(artists, versionOptions ?? VersionOptions.Published, QueryHints.Empty).ToList();
        }

        public IEnumerable<ArtistsContainerPart> GetArtistsContainersByDelimiter(int delimiterId, VersionOptions versionOptions = null) {
            var artists = GetArtistsByDelimiter(delimiterId).Select(x => x.ContainerId);
            return _contentManager.GetMany<ArtistsContainerPart>(artists, versionOptions ?? VersionOptions.Published, QueryHints.Empty).ToList();
        }

        public IQueryable<Artist> GetArtistsByAgent(int agentId) {
            return _artistRepository.Fetch(x => x.Agent.Id == agentId).AsQueryable();
        }

        public IQueryable<Artist> GetArtistsByDelimiter(int delimiterId) {
            return _artistRepository.Fetch(x => x.Delimiter.Id == delimiterId).AsQueryable();
        }

        public void RemoveArtist(Artist artist) {
            if (artist == null)
                return;

            _artistRepository.Delete(artist);
        }

        public void RemoveArtistByAgent(ArtistsContainerPart container, int agentId) {
            var artist = container.Artists.FirstOrDefault(x => x.Agent.Id == agentId);

            RemoveArtist(artist);
        }

        public Artist AddArtist(ArtistsContainerPart container, Agent agent, Delimiter delimiter) {
            var artist = new Artist {
                ContainerId = container.Id,
                Agent = agent,
                Delimiter = delimiter
            };
            _artistRepository.Create(artist);
            return artist;
        }

        public Artist GetArtist(int id) {
            return _artistRepository.Get(id);
        }

        public IEnumerable<Artist> GetArtists(int containerId) {
            var query = from artist in _artistRepository.Fetch(x => x.ContainerId == containerId)
                        select new {
                            record = artist, 
                            agent = artist.Agent  // Eager loading.
                        };
            return query.Select(x => x.record).ToList();
        }

        public void ClearArtists(int containerId, IEnumerable<int> except = null) {
            var query = GetArtists(containerId);

            if (except != null) {
                query = query.Where(x => !except.Contains(x.Id)).ToList();
            }

            foreach (var artist in query) {
                _artistRepository.Delete(artist);
            }
        }

        public void DeleteAgent(Agent agent) {
            _agentRepository.Delete(agent);
        }

        public void DeleteDelimiter(Delimiter delimiter) {
            _delimiterRepository.Delete(delimiter);
        }

        public Agent GetAgentByName(string name) {
            return _agentRepository.Get(x => x.Name == name);
        }
    }
}