using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using BteamMongoDB.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using RationalEvs.Entities;
using RationalEvs.Events;

namespace RationalEvs.Repositories
{
    public class EventsRepository<TEntity, TId> : IEventsRepository<TEntity, TId> where TEntity : IVersionableEntity<TId>, new() where TId : new()
    {
        private readonly IRepository<EntityEventSource<TEntity, TId>, TId> _repository;
        private readonly IQuerySnapshotBuilder<IMongoQuery, TEntity> _mongoBuilder;
        private readonly int _timeOut;
        private readonly bool _storeHistory;
        private readonly int _maxHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository&lt;TEntity&gt;" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mongoBuilder">The mongo builder.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        public EventsRepository(IRepository<EntityEventSource<TEntity, TId>, TId> repository, IQuerySnapshotBuilder<IMongoQuery, TEntity> mongoBuilder, bool storeHistory)
            : this(repository, mongoBuilder, storeHistory, 10)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository&lt;TEntity&gt;" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mongoBuilder">The mongo builder.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        /// <param name="maxHistory">The max history.</param>
        public EventsRepository(IRepository<EntityEventSource<TEntity, TId>, TId> repository, IQuerySnapshotBuilder<IMongoQuery, TEntity> mongoBuilder, bool storeHistory, int maxHistory)
        {
            _repository = repository;
            _mongoBuilder = mongoBuilder;
            _storeHistory = storeHistory;
            _maxHistory = maxHistory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository&lt;TEntity&gt;" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mongoBuilder">The mongo builder.</param>
        /// <param name="timeOut">The time out.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        /// <param name="maxHistory">The max history.</param>
        public EventsRepository(IRepository<EntityEventSource<TEntity, TId>, TId> repository, IQuerySnapshotBuilder<IMongoQuery, TEntity> mongoBuilder, int timeOut = 30, bool storeHistory = false, int maxHistory = 10)
        {
            _repository = repository;
            _mongoBuilder = mongoBuilder;
            _timeOut = timeOut;
            _storeHistory = storeHistory;
            _maxHistory = maxHistory;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository&lt;TEntity&gt;" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mongoBuilder">The mongo builder.</param>
        /// <param name="timeOut">The time out.</param>
        public EventsRepository(IRepository<EntityEventSource<TEntity, TId>, TId> repository, IQuerySnapshotBuilder<IMongoQuery, TEntity> mongoBuilder, int timeOut)
        {
            _repository = repository;
            _mongoBuilder = mongoBuilder;
            _timeOut = timeOut;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, TId> GetEvents(TId id)
        {
            return _repository.FindById(id);
        }

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public IEnumerable<EntityEventSource<TEntity, TId>> FindEntities(object spec)
        {
            return _repository.Find(spec);
        }

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        public IEnumerable<EntityEventSource<TEntity, TId>> FindEntities(Expression<Func<TEntity, bool>> func)
        {
            return _repository.Find(func);
        }


        /// <summary>
        /// Finds the and modify.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, TId> AddEvent(TId id, IEnumerable<IDomainEvent<TEntity>> events)
        {
            _repository.Create(new EntityEventSource<TEntity, TId> { Id = id, Status = "Ready" });
            EntityEventSource<TEntity, TId> entity = null;

            SpinWait.SpinUntil(() =>
            {
                var query = Query.And(Query.EQ("_id", BsonDocumentWrapper.Create(id)),
                    Query.Or(Query.EQ("Status", "Ready"),
                             Query.LTE("ProcessingAt", new BsonDateTime(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeOut)))),
                             Query.EQ("ProcessingBy", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)))
                );

                entity = _repository.FindAndModify
                    (
                    query,
                    x => ((UpdateBuilder)x.GetUpdateBuilder())
                    .PushAll("Events", events.Select(BsonDocumentWrapper.Create))
                    .Set("Status", new BsonString("Processing"))
                    .Set("ProcessingAt", new BsonDateTime(DateTime.UtcNow))
                    .Set("ProcessingBy", new BsonString(Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture))),
                    false,
                    true
                    );
                return entity != null;
            });

            return entity;
        }

        /// <summary>
        /// Finds the and modify.
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="event">The @event.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, TId> AddEvent(TId id, IDomainEvent<TEntity> @event)
        {
            _repository.Create(new EntityEventSource<TEntity, TId> { Id = id, Status = "Ready" });
            EntityEventSource<TEntity, TId> entity = null;

            SpinWait.SpinUntil(() =>
                {
                    var query = Query.And(Query.EQ("_id", BsonDocumentWrapper.Create(id)),
                                          Query.Or(Query.EQ("Status", "Ready"),
                                                   Query.LTE("ProcessingAt",
                                                             new BsonDateTime(
                                                                 DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeOut)))),
                                                   Query.EQ("ProcessingBy",
                                                            Thread.CurrentThread.ManagedThreadId.ToString(
                                                                CultureInfo.InvariantCulture))));

                    entity = _repository.FindAndModify
                        (
                        query,
                        x => ((UpdateBuilder)x.GetUpdateBuilder())
                        .Push("Events", BsonDocumentWrapper.Create(@event))
                        .Set("Status", new BsonString("Processing"))
                        .Set("ProcessingAt", new BsonDateTime(DateTime.UtcNow))
                        .Set("ProcessingBy", new BsonString(Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture))),
                        false,
                        true
                        );
                    return entity != null;
                });

            return entity;
        }


        /// <summary>
        /// Releases the entity.
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseEntity(TId id)
        {
            _repository.Update
                (
                    new { id },
                    x => ((UpdateBuilder)x.GetUpdateBuilder())
                             .Set("Status", new BsonString("Ready"))
                             .Set("ProcessingBy", new BsonString(""))
                );
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="state">The state.</param>
        /// <param name="lastEventApplied">The last event applied.</param>
        /// <param name="appliedEvents">The applied events.</param>
        /// <param name="processedAt">The processed at.</param>
        /// <param name="version">The version.</param>
        public void SaveHistory(TEntity entity, string state, IDomainEvent<TEntity> lastEventApplied, List<IDomainEvent<TEntity>> appliedEvents,
                                DateTime processedAt, long version)
        {
            if (!_storeHistory)
                return;

            var item = new HistoricEntity<TEntity>
            {
                AppliedEvent = appliedEvents,
                ProcessedAt = processedAt,
                State = state ?? "Sin Estado",
                LastEventApplied = lastEventApplied,
                Version = version
            };

            if (_maxHistory > 0)
            {
                var entityEventSource = _repository.FindById(entity.Id);

                if (entityEventSource.History.Count > _maxHistory)
                {
                    _repository.Update
                        (
                            new { entity.Id },
                            x => ((UpdateBuilder)x.GetUpdateBuilder())
                                        .PopFirst("History")
                                        .Push("History", BsonDocumentWrapper.Create(item))
                        );

                    return;
                }
            }

            _repository.Update
                (
                    new { entity.Id },
                    x => ((UpdateBuilder)x.GetUpdateBuilder())
                             .Push("History", BsonDocumentWrapper.Create(item))
                );

        }

        /// <summary>
        /// Saves the snap shot.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="snapShotType">Type of the snap shot.</param>
        /// <param name="events">The events.</param>
        /// <param name="state">The state.</param>
        public void SaveSnapShot(TEntity entity, SnapShotType snapShotType, IEnumerable<IDomainEvent<TEntity>> events, string state)
        {
            _repository.Update
                (
                    _mongoBuilder.GetQuery(snapShotType, entity),
                    x => ((UpdateBuilder)x.GetUpdateBuilder())
                             .Set("SnapShot", BsonDocumentWrapper.Create(entity))
                             .Set("Version", BsonDocumentWrapper.Create(entity.Version))
                             .Set("State", BsonDocumentWrapper.Create(state))
                             .PullAll("Events", events.Select(BsonDocumentWrapper.Create))
                             .Set("AppliedEvents", BsonDocumentWrapper.Create(events))
                );
        }

        /// <summary>
        /// Removes the events.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void RemoveEvents(TEntity entity)
        {
            _repository.Remove(entity.Id);
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        public void RemoveEvent(TId id, IDomainEvent<TEntity> @event)
        {
            var qFind = Query.And
            (
                Query.EQ("_id", BsonDocumentWrapper.Create(id))
            );

            var qUpdate = Query.And
            (
                Query.EQ("_t", BsonDocumentWrapper.Create(@event.GetType().Name)),
                Query.EQ("Version", BsonDocumentWrapper.Create(@event.Version))
            );

            _repository.Update
            (
                qFind,
                x => ((UpdateBuilder)x.GetUpdateBuilder()).Pull("Events", qUpdate)
            );
        }

        /// <summary>
        /// Finds the enity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        public EntityEventSource<TEntity, TId> FindEntity(object spec)
        {
            return _repository.FindOne(spec);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
        }
    }
}