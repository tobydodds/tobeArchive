namespace Tobe.Collection.Models {
    public class Credit {
        public virtual int Id { get; set; }
        public virtual int ContainerId { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual Role Role { get; set; }
    }
}