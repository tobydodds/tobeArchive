namespace Tobe.Collection.Models {
    public class RecordLabel {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public static readonly RecordLabel Empty = new RecordLabel();
    }
}