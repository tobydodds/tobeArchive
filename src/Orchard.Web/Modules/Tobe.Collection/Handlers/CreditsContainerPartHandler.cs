using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Handlers {
    public class CreditsContainerPartHandler : ContentHandler {
        private readonly ICreditsManager _creditsManager;

        public CreditsContainerPartHandler(ICreditsManager creditsManager) {
            _creditsManager = creditsManager;
            OnActivated<CreditsContainerPart>(SetupFields);
            OnRemoved<CreditsContainerPart>(DeleteCredits);
        }

        private void DeleteCredits(RemoveContentContext context, CreditsContainerPart part) {
            _creditsManager.ClearCredits(part.Id);
        }

        private void SetupFields(ActivatedContentContext context, CreditsContainerPart part) {
            part.CreditsField.Loader(x => _creditsManager.GetCredits(part.Id));
        }
    }
}