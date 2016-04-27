using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Handlers {
    public class ArtistsContainerPartHandler : ContentHandler {
        private readonly IArtistsManager _artistsManager;

        public ArtistsContainerPartHandler(IArtistsManager artistsManager) {
            _artistsManager = artistsManager;
            OnActivated<ArtistsContainerPart>(SetupFields);
            OnRemoved<ArtistsContainerPart>(DeleteArtists);
        }

        private void DeleteArtists(RemoveContentContext context, ArtistsContainerPart part) {
            _artistsManager.ClearArtists(part.Id);
        }

        private void SetupFields(ActivatedContentContext context, ArtistsContainerPart part) {
            part.ArtistsField.Loader(x => _artistsManager.GetArtists(part.Id));
        }
    }
}