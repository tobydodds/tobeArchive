using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Tobe.Collection.Models;

namespace Tobe.Collection.Fields {
    public class GenresField : ContentField {
        internal LazyField<IEnumerable<Genre>> _genresField = new LazyField<IEnumerable<Genre>>();

        public IEnumerable<Genre> Genres {
            get { return _genresField.Value; }
        } 
    }
}