namespace Tobe.Collection.Models {
    public class Length {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public static readonly Length Empty = new Length();
    }
}