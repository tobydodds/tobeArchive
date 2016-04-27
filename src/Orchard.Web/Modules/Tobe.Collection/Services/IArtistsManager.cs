using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface IArtistsManager : IDependency {
        IEnumerable<Agent> GetAgents(IEnumerable<int> ids = null);
        IEnumerable<Delimiter> GetDelimiters(IEnumerable<int> ids = null);
        IEnumerable<ArtistsContainerPart> GetArtistsContainersByAgent(int agentId, VersionOptions versionOptions = null);
        IEnumerable<ArtistsContainerPart> GetArtistsContainersByDelimiter(int delimiterId, VersionOptions versionOptions = null);
        IQueryable<Artist> GetArtistsByAgent(int agentId);
        IQueryable<Artist> GetArtistsByDelimiter(int delimiterId);
        void RemoveArtist(Artist artist);
        void RemoveArtistByAgent(ArtistsContainerPart container, int agentId);
        Artist AddArtist(ArtistsContainerPart container, Agent agent, Delimiter delimiter);
        Artist GetArtist(int id);
        IEnumerable<Artist> GetArtists(int containerId);
        void ClearArtists(int containerId, IEnumerable<int> except = null);
        void DeleteAgent(Agent agent);
        void DeleteDelimiter(Delimiter delimiter);
        Agent GetAgentByName(string name);
    }
}