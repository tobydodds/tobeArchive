﻿using System;
using System.Linq;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.UI.Resources;
using Tobe.Collection.Helpers;
using Orchard.Utility.Extensions;

namespace Tobe.Collection.Handlers {
    public class AlbumPartHandler : ContentHandler {
        private readonly IAlbumManager _albumManager;
        private readonly IResourceManager _resourceManager;

        public AlbumPartHandler(
            IRepository<AlbumPartRecord> repository,
            IAlbumManager albumManager,
            IResourceManager resourceManager) {

            _albumManager = albumManager;
            _resourceManager = resourceManager;
            Filters.Add(StorageFilter.For(repository));
            OnIndexing<AlbumPart>(Index);
            OnActivated<AlbumPart>(SetupArtistFields);
            OnActivated<AlbumPart>(SetupCreditFields);
        }

        private void SetupArtistFields(ActivatedContentContext context, AlbumPart part) {
            part._allArtistsField.Loader(() => _albumManager.GetAllAgentArtistsFor(part).ToList());
        }

        private void SetupCreditFields(ActivatedContentContext context, AlbumPart part) {
            part._allAgentsField.Loader(() => _albumManager.GetAllAgentCreditsFor(part).ToList());
        }

        private void Index(IndexContentContext context, AlbumPart part) {

            if (!String.IsNullOrWhiteSpace(part.Description)) {
                context.DocumentIndex.Add("album-description", part.Description).Analyze().Store();
            }

            if (!String.IsNullOrWhiteSpace(part.CatalogNumber)) {
                context.DocumentIndex.Add("album-catalognumber", part.CatalogNumber).Analyze().Store();
            }

            var artistsPart = part.As<ArtistsContainerPart>();
            if (artistsPart != null) {
                foreach (var credit in artistsPart.Artists) {
                    context.DocumentIndex.Add("album-artists", credit.Agent.Name).Analyze().Store();
                }
            }

            var creditsPart = part.As<CreditsContainerPart>();
            if (creditsPart != null) {
                foreach (var credit in creditsPart.Credits) {
                    context.DocumentIndex.Add("album-credits", credit.Agent.Name).Analyze().Store();
                }
            }
        }
    }
}