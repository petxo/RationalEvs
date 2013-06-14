using RationalEvs.Events;

namespace RationalEvs.Sql.Test.DomainFake.Events
{
    public class IcebergEvent : IDomainEvent<Ship>
    {

        public long Version
        {
            get;
            set;
        }

        public void Apply(Ship root)
        {
            
        }

        public bool CanApply(Ship root)
        {
            return true;
        }
    }
}