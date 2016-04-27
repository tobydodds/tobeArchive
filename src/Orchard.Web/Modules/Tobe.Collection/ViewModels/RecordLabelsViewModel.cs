using System.Collections.Generic;
using Tobe.Collection.Fields;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class RecordLabelsViewModel {
        public RecordLabelsViewModel() {
            RecordLabels = new List<RecordLabelViewModel>();
        }
        public IList<RecordLabelViewModel> RecordLabels { get; set; }
        public IList<RecordLabel> AllRecordLabels { get; set; }
        public RecordLabelsField ContentField { get; set; }
    }
}