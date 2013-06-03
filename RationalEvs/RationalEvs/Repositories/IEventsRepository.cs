using System.Linq.Expressions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using RationalEvs.Entities;
using RationalEvs.Events;

namespace RationalEvs.Repositories
{
    public interface IEventsRepository<TEntity, TId> : IDisposable where TEntity : IVersionableEntity<TId>, new()
    {
        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        EntityEventSource<TEntity, TId> GetEvents(TId id);

        /// <summary>
        /// Finds the and modify.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        EntityEventSource<TEntity, TId> AddEvent(TId id, IEnumerable<IDomainEvent<TEntity>> events);

        /// <summary>
        /// Finds the and modify.
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="event">The @event.</param>
        /// <returns></returns>
        EntityEventSource<TEntity, TId> AddEvent(TId id, IDomainEvent<TEntity> @event);

        /// <summary>
        /// Removes the events.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void RemoveEvents(TEntity entity);


        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        void RemoveEvent(TId id, IDomainEvent<TEntity> @event);

        /// <summary>
        /// Saves the snap shot.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="query">The query.</param>
        /// <param name="events">The events.</param>
        /// <param name="state">The state.</param>
        void SaveSnapShot(TEntity entity, IMongoQuery query, IEnumerable<IDomainEvent<TEntity>> events, string state);


        /// <summary>
        /// Finds the enity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        EntityEventSource<TEntity, TId> FindEntity(object spec);

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        IEnumerable<EntityEventSource<TEntity, TId>> FindEntities(object spec);

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        IEnumerable<EntityEventSource<TEntity, TId>> FindEntities(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// Releases the entity.
        /// </summary>
        /// <param name="id"></param>
        void ReleaseEntity(TId id);

        /// <summary>
        /// Saves the history.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="state">The state.</param>
        /// <param name="lastEventApplied">The last event applied.</param>
        /// <param name="appliedEvents">The applied events.</param>
        /// <param name="processedAt">The processed at.</param>
        /// <param name="version">The version.</param>
        void SaveHistory(TEntity entity, string state, IDomainEvent<TEntity> lastEventApplied,
                         List<IDomainEvent<TEntity>> appliedEvents,
                         DateTime processedAt, long version);
    }
}