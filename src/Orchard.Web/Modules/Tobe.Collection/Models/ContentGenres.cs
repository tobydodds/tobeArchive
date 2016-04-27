using Orchard.ContentManagement.Records;

namespace Tobe.Collection.Models {
    public class ContentGenres {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord ContentItem { get; set; }
        public virtual string FieldName { get; set; }
        public virtual Genre Genre { get; set; }
    }
}