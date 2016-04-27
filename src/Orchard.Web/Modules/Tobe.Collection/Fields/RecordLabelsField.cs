using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Tobe.Collection.Models;

namespace Tobe.Collection.Fields {
    public class RecordLabelsField : ContentField {
        internal LazyField<IEnumerable<RecordLabel>> _recordLabelsField = new LazyField<IEnumerable<RecordLabel>>();

        public IEnumerable<RecordLabel> RecordLabels {
            get { return _recordLabelsField.Value; }
        } 
    }
}