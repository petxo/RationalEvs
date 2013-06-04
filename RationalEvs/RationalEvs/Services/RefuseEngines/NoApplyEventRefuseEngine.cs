using RationalEvs.Events;

namespace RationalEvs.Services.RefuseEngines
{
    public class NoApplyEventRefuseEngine<TEntity, TId> : IRefuseEngine<TEntity, TId> where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Refuses the specified root.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="root">The root.</param>
        /// <param name="event">The @event.</param>
        public void Refuse(IAggregateRoot<TEntity, TId> root, IDomainEvent<TEntity> @event)
        {
            root.RemoveFromAppliedEvents(@event);
            root.Regenerate();
        }
    }
}