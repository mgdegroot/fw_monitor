using System;
using System.Runtime.Serialization;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public abstract class Config : IConfig
    {
        [DataMember(Order=0)]
        public virtual string Name { get; set; }
        [DataMember(Order=1)]
        public virtual string Description { get; set; } = string.Empty;
        
        public abstract ICreator Creator { get; set; }

        public abstract override string ToString();
        public override bool Equals(Object obj) => obj?.ToString() == ToString();
        public abstract override int GetHashCode();
        

    }
}