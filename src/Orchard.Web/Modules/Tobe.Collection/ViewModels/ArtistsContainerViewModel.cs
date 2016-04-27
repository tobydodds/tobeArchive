using System.Collections.Generic;
using System.Data.SqlClient;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class ArtistsContainerViewModel {
        public IEnumerable<ArtistViewModel> Artists { get; set; }
        public IEnumerable<Delimiter> AllDelimiters { get; set; }
    }
}