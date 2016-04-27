using System;
using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Helpers;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Indexing;
using Orchard.Utility.Extensions;

namespace Tobe.Collection.Services {
    public class AlbumManager : IAlbumManager {
        private readonly IContentManager _contentManager;
        private readonly IRepository<AmgRating> _amgRatingRepository;
        private readonly IRepository<Format> _formatRepository;
        private readonly IRepository<Length> _lengthRepository;
        private readonly IArtistsManager _artistsManager;
        private readonly ICreditsManager _creditsManager;

        public AlbumManager(
            IContentManager contentManager, 
            IRepository<AmgRating> amgRatingRepository,
            IRepository<Format> formatRepository,
            IRepository<Length> lengthRepository,
            IArtistsManager artistsManager,
            ICreditsManager creditsManager) {

            _contentManager = contentManager;
            _amgRatingRepository = amgRatingRepository;
            _formatRepository = formatRepository;
            _lengthRepository = lengthRepository;
            _artistsManager = artistsManager;
            _creditsManager = creditsManager;
        }

        public AlbumPart GetAlbum(int id, VersionOptions versionOptions) {
            return _contentManager.Get<AlbumPart>(id, versionOptions);
        }

        public IEnumerable<Agent> GetAllAgentArtistsFor(AlbumPart album) {
            return HarvestAllAgentArtistsFor(album).Distinct();
        }

        public IEnumerable<AlbumAgentArtistRelation> GetAllAlbumArtistsByAgent(int agentId, VersionOptions versionOptions = null) {
            return HarvestAllAlbumArtistsByAgent(agentId, versionOptions).Distinct(new AlbumAgentArtistRelationComparer());
        }

        private IEnumerable<Agent> HarvestAllAgentArtistsFor(AlbumPart album) {

            var artistsContainerPart = album.As<ArtistsContainerPart>();

            if (artistsContainerPart != null) {
                foreach (var agent in artistsContainerPart.Artists.Select(x => x.Agent)) {
                    yield return agent;
                }
            }
        }

        private IEnumerable<AlbumAgentArtistRelation> HarvestAllAlbumArtistsByAgent(int agentId, VersionOptions versionOptions = null) {
            if (versionOptions == null)
                versionOptions = VersionOptions.Published;

            // Albums by Album Artists
            var albumArtists = _artistsManager.GetArtistsByAgent(agentId).ToArray();
            var albumsByArtists = _contentManager.GetMany<AlbumPart>(albumArtists.Select(x => x.ContainerId), versionOptions, QueryHints.Empty).ToDictionary(x => x.Id);

            foreach (var artist in albumArtists.Where(x => albumsByArtists.ContainsKey(x.ContainerId))) {
                var album = albumsByArtists[artist.ContainerId];
                yield return new AlbumAgentArtistRelation(album);
            }
        }
        
        public IEnumerable<Agent> GetAllAgentCreditsFor(AlbumPart album) {
            return HarvestAllAgentCreditsFor(album).Distinct();
        }

        public IEnumerable<AlbumAgentCreditRelation> GetAllAlbumCreditsByAgent(int agentId, VersionOptions versionOptions = null) {
            return HarvestAllAlbumCreditsByAgent(agentId, versionOptions).Distinct(new AlbumAgentCreditRelationComparer());
        }

        private IEnumerable<Agent> HarvestAllAgentCreditsFor(AlbumPart album) {

            var creditsContainerPart = album.As<CreditsContainerPart>();

            if (creditsContainerPart != null) {
                foreach (var agent in creditsContainerPart.Credits.Select(x => x.Agent)) {
                    yield return agent;
                }
            }
        }

        private IEnumerable<AlbumAgentCreditRelation> HarvestAllAlbumCreditsByAgent(int agentId, VersionOptions versionOptions = null) {
            if (versionOptions == null)
                versionOptions = VersionOptions.Published;

            // Albums by Album Credits
            var albumCredits = _creditsManager.GetCreditsByAgent(agentId).ToArray();
            var albumsByCredits = _contentManager.GetMany<AlbumPart>(albumCredits.Select(x => x.ContainerId), versionOptions, QueryHints.Empty).ToDictionary(x => x.Id);

            foreach (var credit in albumCredits.Where(x => albumsByCredits.ContainsKey(x.ContainerId))) {
                var album = albumsByCredits[credit.ContainerId];
                yield return new AlbumAgentCreditRelation(album, credit.Role.Name);
            }
        }

        public AmgRating GetAmgRating(int id) {
            return _amgRatingRepository.Get(id);
        }

        public AmgRating GetAmgRatingByName(string name) {
            return _amgRatingRepository.Get(x => x.Name == name);
        }

        public IEnumerable<AmgRating> GetAmgRatings() {
            return _amgRatingRepository.Table.OrderBy(x => x.Name);
        }

        public Format GetFormat(int id) {
            return _formatRepository.Get(id);
        }

        public Format GetFormatByName(string name) {
            return _formatRepository.Get(x => x.Name == name);
        }

        public IEnumerable<Format> GetFormats() {
            return _formatRepository.Table.OrderBy(x => x.Name);
        }

        public Length GetLength(int id) {
            return _lengthRepository.Get(id);
        }

        public Length GetLengthByName(string name) {
            return _lengthRepository.Get(x => x.Name == name);
        }

        public IEnumerable<Length> GetLengths() {
            return _lengthRepository.Table.OrderBy(x => x.Name);
        }

        public IEnumerable<AlbumPart> GetAlbumsByAmgRating(int amgRatingId, VersionOptions versionOptions = null) {
            return _contentManager
                .Query<AlbumPart, AlbumPartRecord>(versionOptions)
                .Where(x => x.AmgRating.Id == amgRatingId)
                .Join<TitlePartRecord>()
                .OrderBy(x => x.Title)
                .List();
        }    
        public IEnumerable<AlbumPart> GetAlbumsByFormat(int formatId, VersionOptions versionOptions = null) {
            return _contentManager
                .Query<AlbumPart, AlbumPartRecord>(versionOptions)
                .Where(x => x.Format.Id == formatId)
                .Join<TitlePartRecord>()
                .OrderBy(x => x.Title)
                .List();
        } 
        public IEnumerable<AlbumPart> GetAlbumsByLength(int lengthId, VersionOptions versionOptions = null) {
            return _contentManager
                .Query<AlbumPart, AlbumPartRecord>(versionOptions)
                .Where(x => x.Length.Id == lengthId)
                .Join<TitlePartRecord>()
                .OrderBy(x => x.Title)
                .List();
        } 
    }
}