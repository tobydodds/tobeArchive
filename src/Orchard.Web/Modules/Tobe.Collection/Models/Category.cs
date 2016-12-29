namespace Tobe.Collection.Models {
    public class Category {
        public virtual int Id { get; set; }
        public virtual int ContainerId { get; set; }
        public virtual Genre Genre { get; set; }

    }
}