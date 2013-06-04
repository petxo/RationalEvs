using System;
using System.Collections.Generic;
using RationalEvs.Events;

namespace RationalEvs.Services
{
    public interface IStateStrategy<TEntity, TId>
        where TEntity : IVersionableEntity<TId>, new()
    {
        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        TEntity FindEntity(object spec);

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        IEnumerable<Type> GetIntermediateEvents(TId id, string finalState);

        void ApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> domainEvents);

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="domainEvent">The domain event.</param>
        AggregateRoot<TEntity, TId> AddEvent(TId id, IDomainEvent<TEntity> domainEvent);

        AggregateRoot<TEntity, TId> AddEvents(TId id, IEnumerable<IDomainEvent<TEntity>> domainEvents);

        /// <summary>
        /// Occurs when [action state notification].
        /// </summary>
        event ActionStateNotificationHandler<TEntity> ActionStateNotification;

        /// <summary>
        /// Occurs when [enity not exist notification].
        /// </summary>
        event EnityNotExistNotificationHandler<TId> EnityNotExistNotification;

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="domainEvent">The domain event.</param>
        void ApplyEvent(TId id, IDomainEvent<TEntity> domainEvent);

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The id.</param>
        void GetEntity(TId id);
    }
}