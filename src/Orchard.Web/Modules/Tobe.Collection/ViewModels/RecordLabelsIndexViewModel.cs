using System.Collections.Generic;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class RecordLabelsIndexViewModel {
        public IEnumerable<RecordLabel> RecordLabels { get; set; }
        public dynamic Pager { get; set; }
    }
}