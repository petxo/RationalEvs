using RationalEvs.Events;

namespace RationalEvs.Test.DomainFake.Events
{
    public class DepartureEvent : IDomainEvent<Ship>
    {
        public Port Port { get; set; }
        public long Version { get; set; }

        public void Apply(Ship root)
        {
            root.Port = Port.AtSea;
        }

        public bool CanApply(Ship root)
        {
            return root.Port.Equals(Port);
        }
    }
}