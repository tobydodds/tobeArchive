using Orchard.ContentManagement.Records;

namespace Tobe.Collection.Models {
    public class ContentRecordLabels {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord ContentItem { get; set; }
        public virtual string FieldName { get; set; }
        public virtual RecordLabel RecordLabel { get; set; }
    }
}