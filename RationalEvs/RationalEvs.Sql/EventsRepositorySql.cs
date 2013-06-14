using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using Bteam.NHibernate;
using Bteam.NHibernate.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using RationalEvs.Entities;
using RationalEvs.Events;
using RationalEvs.Repositories;
using MongoDB.Bson;

namespace RationalEvs.Sql
{
    public class EventsRepositorySql<TEntity> : Repository<EntityEventSourceWrapper, long>, IEventsRepository<TEntity, long>
        where TEntity : IVersionableEntity<long>, new()
    {
        private readonly int _timeOut;
        private readonly bool _storeHistory;
        private readonly int _maxHistory;


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, bool storeHistory)
            : this(nHibernateHelper, storeHistory, 10)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        /// <param name="maxHistory">The max history.</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, bool storeHistory, int maxHistory)
            : base(nHibernateHelper)
        {
            _storeHistory = storeHistory;
            _maxHistory = maxHistory;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="timeOut">The time out.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        /// <param name="maxHistory">The max history.</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, int timeOut = 30, bool storeHistory = false, int maxHistory = 10) 
            : base(nHibernateHelper)
        {
            _timeOut = timeOut;
            _storeHistory = storeHistory;
            _maxHistory = maxHistory;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="timeOut">The time out.</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, int timeOut) 
            : base(nHibernateHelper)
        {
            _timeOut = timeOut;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, long> GetEvents(long id)
        {
            return NHibernateHelper.CurrentSession.Get<EntityEventSource<TEntity, long>>(id);
        }

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public IEnumerable<EntityEventSource<TEntity, long>> FindEntities(object spec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the events by alternate key.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        public IEnumerable<EntityEventSource<TEntity, long>> FindEntities(Expression<Func<TEntity, bool>> func)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Finds the and modify.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, long> AddEvent(long id, IEnumerable<IDomainEvent<TEntity>> events)
        {
            NHibernateHelper.CurrentSession.Save(new EntityEventSourceWrapper {Id = id, Status = "Ready"});
            EntityEventSource<TEntity, long> entity = null;

            SpinWait.SpinUntil(() =>
            {
                var criterion = Restrictions.Or(Restrictions.Or(Restrictions.Eq("Status", "Ready"), 
                            Restrictions.Le("ProcessingAt", DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeOut)))), 
                            Restrictions.Eq("ProcessingBy", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)));

                var query = Restrictions.And(Restrictions.Eq("Id", id), criterion);
                var entitySourceWrapper = (EntityEventSourceWrapper) NHibernateHelper.CurrentSession
                                            .CreateCriteria<EntityEventSourceWrapper>()
                                            .Add(query).UniqueResult();

                if (entitySourceWrapper != null)
                {
                    entitySourceWrapper.Status = "Processing";
                    entitySourceWrapper.ProcessingAt = DateTime.UtcNow;
                    entitySourceWrapper.ProcessingBy = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);

                    //Todo Translate to entity
                }
                
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
        public EntityEventSource<TEntity, long> AddEvent(long id, IDomainEvent<TEntity> @event)
        {
            if (NHibernateHelper.CurrentSession.Get<EntityEventSourceWrapper>(id) == null)
            {
                NHibernateHelper.CurrentSession.Save(new EntityEventSourceWrapper { Id = id, Status = "Ready" });
                NHibernateHelper.CurrentSession.Flush();
            }

            EntityEventSource<TEntity, long> entity = null;

            SpinWait.SpinUntil(() =>
            {
                var criterion = Restrictions.Or(Restrictions.Or(Restrictions.Eq("Status", "Ready"),
                            Restrictions.Le("ProcessingAt", DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeOut)))),
                            Restrictions.Eq("ProcessingBy", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)));

                var query = Restrictions.And(Restrictions.Eq("Id", id), criterion);
                var entitySourceWrapper = (EntityEventSourceWrapper)NHibernateHelper.CurrentSession
                                            .CreateCriteria<EntityEventSourceWrapper>()
                                            .Add(query).UniqueResult();

                if (entitySourceWrapper != null)
                {
                    entitySourceWrapper.Status = "Processing";
                    entitySourceWrapper.ProcessingAt = DateTime.UtcNow;
                    entitySourceWrapper.ProcessingBy = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);

                    var eventWrapper = new EventWrapper
                                           {
                                               Type = @event.GetType().FullName, Data = @event.ToBson()
                                           };
                    entitySourceWrapper.AddEvent(eventWrapper);

                    NHibernateHelper.CurrentSession.Update(entitySourceWrapper);
                    NHibernateHelper.CurrentSession.Flush();

                    entity = new EntityEventSource<TEntity, long>();
                    Translator.Instance.TranslateToDomain(entity, entitySourceWrapper);
                }

                return entity != null;
            });

            return entity;
        }


        /// <summary>
        /// Releases the entity.
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseEntity(long id)
        {
            throw new NotImplementedException();
//            _repository.Update
//                (
//                    new { id },
//                    x => ((UpdateBuilder)x.GetUpdateBuilder())
//                             .Set("Status", BsonString.Create("Ready"))
//                             .Set("ProcessingBy", BsonString.Create(""))
//                );
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
            throw new NotImplementedException();
//            if (!_storeHistory)
//                return;
//
//            var item = new HistoricEntity<TEntity>
//            {
//                AppliedEvent = appliedEvents,
//                ProcessedAt = processedAt,
//                State = state ?? "Sin Estado",
//                LastEventApplied = lastEventApplied,
//                Version = version
//            };
//
//            if (_maxHistory > 0)
//            {
//                var entityEventSource = _repository.FindById(entity.Id);
//
//                if (entityEventSource.History.Count > _maxHistory)
//                {
//                    _repository.Update
//                        (
//                            new { entity.Id },
//                            x => ((UpdateBuilder)x.GetUpdateBuilder())
//                                        .PopFirst("History")
//                                        .Push("History", BsonDocumentWrapper.Create(item))
//                        );
//
//                    return;
//                }
//            }
//
//            _repository.Update
//                (
//                    new { entity.Id },
//                    x => ((UpdateBuilder)x.GetUpdateBuilder())
//                             .Push("History", BsonDocumentWrapper.Create(item))
//                );

        }

        /// <summary>
        /// Saves the snap shot.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="events">The events.</param>
        /// <param name="state">The state.</param>
        /// <param name="query">The query.</param>
        public void SaveSnapShot(TEntity entity, IMongoQuery query, IEnumerable<IDomainEvent<TEntity>> events, string state)
        {
            throw new NotImplementedException();
//            _repository.Update
//                (
//                    query,
//                    x => ((UpdateBuilder)x.GetUpdateBuilder())
//                             .Set("SnapShot", BsonDocumentWrapper.Create(entity))
//                             .Set("Version", BsonDocumentWrapper.Create(entity.Version))
//                             .Set("State", BsonDocumentWrapper.Create(state))
//                             .PullAll("Events", events.Select(BsonDocumentWrapper.Create))
//                             .Set("AppliedEvents", BsonDocumentWrapper.Create(events))
//                );
        }

        /// <summary>
        /// Removes the events.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void RemoveEvents(TEntity entity)
        {
            throw new NotImplementedException();
//            _repository.Remove(entity.Id);
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        public void RemoveEvent(long id, IDomainEvent<TEntity> @event)
        {
            throw new NotImplementedException();
//            var qFind = Query.And
//            (
//                Query.EQ("_id", BsonDocumentWrapper.Create(id))
//            );
//
//            var qUpdate = Query.And
//            (
//                Query.EQ("_t", BsonDocumentWrapper.Create(@event.GetType().Name)),
//                Query.EQ("Version", BsonDocumentWrapper.Create(@event.Version))
//            );
//
//            _repository.Update
//            (
//                qFind,
//                x => ((UpdateBuilder)x.GetUpdateBuilder()).Pull("Events", qUpdate)
//            );
        }

        /// <summary>
        /// Finds the enity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        public EntityEventSource<TEntity, long> FindEntity(object spec)
        {
            throw new NotImplementedException();
//            return _repository.FindOne(spec);
        }

    }
}