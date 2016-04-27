namespace Tobe.Collection.Models {
    public class Format {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public static readonly Format Empty = new Format();
    }
}