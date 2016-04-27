using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Tobe.Collection.Models {
    public class CreditsContainerPart : ContentPart {
        internal LazyField<IEnumerable<Credit>> CreditsField = new LazyField<IEnumerable<Credit>>();

        public IEnumerable<Credit> Credits {
            get { return CreditsField.Value; }
        }
    }
}