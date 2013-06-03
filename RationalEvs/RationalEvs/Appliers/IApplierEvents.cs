using System.Collections.Generic;
using MongoDB.Driver;
using RationalEvs.Events;

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
        /// Gets the query snap shot.
        /// </summary>
        IMongoQuery GetQuerySnapShot<TEntity, TId>(TEntity entity) where TEntity : IVersionableEntity<TId>;
    }
}