using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Tobe.Collection.Models {
    public class ArtistsContainerPart : ContentPart {
        internal LazyField<IEnumerable<Artist>> ArtistsField = new LazyField<IEnumerable<Artist>>();

        public IEnumerable<Artist> Artists {
            get { return ArtistsField.Value; }
        }
    }
}