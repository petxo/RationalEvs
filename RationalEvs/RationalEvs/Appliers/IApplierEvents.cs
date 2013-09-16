using System.Collections.Generic;
using MongoDB.Driver;
using RationalEvs.Events;
using RationalEvs.Repositories;

namespace RationalEvs.Appliers
{
    public interface IApplierEvents
    {
        /// <summary>
        /// Gets the ordered events.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="events">The events.</param>
        /// <param name="version"> </param>
        /// <returns></returns>
        IEnumerable<IDomainEvent<TEntity>> GetOrderedEvents<TEntity>(IEnumerable<IDomainEvent<TEntity>> events, long version);


        /// <summary>
        /// Gets the type snap shot.
        /// </summary>
        /// <value>
        /// The type snap shot.
        /// </value>
        SnapShotType TypeSnapShot { get; }

        /// <summary>
        /// Gets the invalid events.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="events">The events.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        IEnumerable<IDomainEvent<TEntity>> GetInvalidEvents<TEntity>(IEnumerable<IDomainEvent<TEntity>> events, long version);
    }
}