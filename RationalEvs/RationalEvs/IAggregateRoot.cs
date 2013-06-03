using System.Collections.Generic;
using System;
using RationalEvs.Events;

namespace RationalEvs
{
    public interface IAggregateRoot<TEntity, TId> : IRoot<TEntity, TId> where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Restores the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        void ApplyEvents(IEnumerable<IDomainEvent<TEntity>> events);

        /// <summary>
        /// Creates the snap shot.
        /// </summary>
        void CreateSnapShot();

        /// <summary>
        /// Restores the specified events.
        /// </summary>
        /// <param name="event">The @event.</param>
        void ApplyEvent(IDomainEvent<TEntity> @event);

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="event">The @event.</param>
        void RemoveEvent(IDomainEvent<TEntity> @event);

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        IEnumerable<Type> GetIntermediateEvents(string finalState);

        /// <summary>
        /// Occurs when [domain event erroneo].
        /// </summary>
        event DomainEventErroneoEventHandler<TEntity> DomainEventErroneo;

        /// <summary>
        /// Removes from applied events.
        /// </summary>
        /// <param name="event">The @event.</param>
        void RemoveFromAppliedEvents(IDomainEvent<TEntity> @event);

        /// <summary>
        /// Regenerates this instance.
        /// </summary>
        void Regenerate();

        /// <summary>
        /// Gets the applied events.
        /// </summary>
        /// <value>The applied events.</value>
        IList<IDomainEvent<TEntity>> AppliedEvents { get; }

        /// <summary>
        /// Saves the history.
        /// </summary>
        void SaveHistory();
    }
}