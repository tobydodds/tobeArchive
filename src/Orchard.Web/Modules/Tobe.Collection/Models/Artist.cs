namespace Tobe.Collection.Models {
    public class Artist {
        public virtual int Id { get; set; }
        public virtual int ContainerId { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual Delimiter Delimiter { get; set; }

    }
}