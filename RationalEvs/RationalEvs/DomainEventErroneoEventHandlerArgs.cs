using RationalEvs.Events;

namespace RationalEvs
{
    public class DomainEventErroneoEventHandlerArgs<TEntity>
    {
        public IDomainEvent<TEntity> Event { get; set; }
    }
}