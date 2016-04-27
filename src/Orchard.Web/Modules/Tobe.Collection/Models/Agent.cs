using System;

namespace Tobe.Collection.Models {
    public class Agent {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameSort { get; set; }
        public virtual string FileUnder { get; set; }
    }
}