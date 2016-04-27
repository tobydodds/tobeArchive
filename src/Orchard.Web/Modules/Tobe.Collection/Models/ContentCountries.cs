using Orchard.ContentManagement.Records;

namespace Tobe.Collection.Models {
    public class ContentCountries {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord ContentItem { get; set; }
        public virtual string FieldName { get; set; }
        public virtual Country Country { get; set; }
    }
}