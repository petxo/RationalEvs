using System;
using System.Collections.Generic;
using RationalEvs.Events;

namespace RationalEvs.Factories
{
    public interface IAggregateFactory<TEntity, TId> where TEntity : IVersionableEntity<TId>, new()
    {

        /// <summary>
        /// Creates the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> GetById(TId id);

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> GetAndApplyEvent(TId id, IDomainEvent<TEntity> @event);

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        /// <param name="domainEventErroneoAction">The domain event erroneo action.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> GetAndApplyEvent(TId id, IDomainEvent<TEntity> @event, Action<AggregateRoot<TEntity, TId>, IDomainEvent<TEntity>> domainEventErroneoAction);

        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> FindEntity(object spec);

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The @event.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> GetAndApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> events);

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The @event.</param>
        /// <param name="domainEventErroneoAction">The domain event erroneo action.</param>
        /// <returns></returns>
        AggregateRoot<TEntity, TId> GetAndApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> events, Action<AggregateRoot<TEntity, TId>, IDomainEvent<TEntity>> domainEventErroneoAction);

    }
}