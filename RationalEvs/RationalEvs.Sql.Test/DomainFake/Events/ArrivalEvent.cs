using RationalEvs.Events;

namespace RationalEvs.Sql.Test.DomainFake.Events
{
    public class ArrivalEvent : IDomainEvent<Ship>
    {
        public Port Port { get; set; }
        public long Version { get; set; }
        public void Apply(Ship root)
        {
            root.Port = Port;
        }

        public bool CanApply(Ship root)
        {
            return true;
        }
    }
}