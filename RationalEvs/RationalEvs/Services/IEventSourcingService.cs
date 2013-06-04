using System;
using System.Collections.Generic;
using RationalEvs.Events;

namespace RationalEvs.Services
{
    public interface IEventSourcingService<TEntity, TId>
        where TEntity : IVersionableEntity<TId>, new()
    {

        /// <summary>
        ///   Occurs when [action state notification].
        /// </summary>
        event ActionStateNotificationHandler<TEntity> ActionStateNotification;

        /// <summary>
        /// Occurs when [enity not exist notification].
        /// </summary>
        event EnityNotExistNotificationHandler<TId> EnityNotExistNotification;

        /// <summary>
        ///   Adds the event.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <param name="domainEvent"> The domain event. </param>
        void AddEvent(TId id, IDomainEvent<TEntity> domainEvent);

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The id.</param>
        void GetEntity(TId id);

        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        TEntity FindEntity(object spec);

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        IEnumerable<Type> GetIntermediateEvents(TId id, string finalState);
    }
}