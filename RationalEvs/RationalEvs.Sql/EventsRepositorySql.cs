using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using Bteam.NHibernate;
using Bteam.NHibernate.Repository;
using MongoDB.Bson.Serialization;
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
        private readonly IQuerySnapshotBuilder<ICriterion, TEntity> _querySnapshotBuilder;
        private readonly bool _storeHistory;
        private readonly int _maxHistory;


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;" /> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="querySnapshotBuilder">The query snapshot builder.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, IQuerySnapshotBuilder<ICriterion, TEntity> querySnapshotBuilder, bool storeHistory)
            : this(nHibernateHelper, querySnapshotBuilder, storeHistory, 10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="storeHistory">if set to <c>true</c> [store history].</param>
        /// <param name="maxHistory">The max history.</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, IQuerySnapshotBuilder<ICriterion, TEntity> querySnapshotBuilder, bool storeHistory, int maxHistory)
            : base(nHibernateHelper)
        {
            _querySnapshotBuilder = querySnapshotBuilder;
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
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, IQuerySnapshotBuilder<ICriterion, TEntity> querySnapshotBuilder, int timeOut = 30, bool storeHistory = false, int maxHistory = 10)
            : base(nHibernateHelper)
        {
            _querySnapshotBuilder = querySnapshotBuilder;
            _timeOut = timeOut;
            _storeHistory = storeHistory;
            _maxHistory = maxHistory;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepositorySql&lt;TEntity, long&gt;"/> class.
        /// </summary>
        /// <param name="nHibernateHelper">The n hibernate helper.</param>
        /// <param name="timeOut">The time out.</param>
        public EventsRepositorySql(INHibernateHelper nHibernateHelper, IQuerySnapshotBuilder<ICriterion, TEntity> querySnapshotBuilder, int timeOut)
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

                    foreach (var domainEvent in events)
                    {
                        BsonClassMap.IsClassMapRegistered(domainEvent.GetType());

                        var eventWrapper = new EventWrapper
                        {
                            Type = domainEvent.GetType().Name,
                            Data = domainEvent.ToBson(),
                            Version = domainEvent.Version
                        };
                        entitySourceWrapper.AddEvent(eventWrapper);
                    }


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
        /// Finds the and modify.
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="event">The @event.</param>
        /// <returns></returns>
        public EntityEventSource<TEntity, long> AddEvent(long id, IDomainEvent<TEntity> @event)
        {
            return AddEvent(id, new[] { @event });
        }


        /// <summary>
        /// Releases the entity.
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseEntity(long id)
        {
            var entityEventSourceWrapper = NHibernateHelper.CurrentSession.Get<EntityEventSourceWrapper>(id);
            entityEventSourceWrapper.Status = "Ready";
            entityEventSourceWrapper.ProcessingBy = string.Empty;
            NHibernateHelper.CurrentSession.Update(entityEventSourceWrapper);
            NHibernateHelper.CurrentSession.Flush();
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
        }

        /// <summary>
        /// Saves the snap shot.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="snapShotType">Type of the snap shot.</param>
        /// <param name="events">The events.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SaveSnapShot(TEntity entity, SnapShotType snapShotType, IEnumerable<IDomainEvent<TEntity>> events, string state)
        {

            var query = _querySnapshotBuilder.GetQuery(snapShotType, entity);

            var entitySourceWrapper = (EntityEventSourceWrapper)NHibernateHelper.CurrentSession
                                            .CreateCriteria<EntityEventSourceWrapper>()
                                            .Add(query).UniqueResult();
            entitySourceWrapper.SnapShot = entity.ToBson();
            entitySourceWrapper.Version = entity.Version;
            entitySourceWrapper.State = state;
            
            entitySourceWrapper.ClearEvents();

            foreach (var domainEvent in events)
            {
                var eventWrapper = new EventWrapper
                {
                    Type = domainEvent.GetType().Name,
                    Data = domainEvent.ToBson()
                };
                entitySourceWrapper.AddEvent(eventWrapper);
            }
            NHibernateHelper.CurrentSession.Update(entitySourceWrapper);
            NHibernateHelper.CurrentSession.Flush();
        }

        /// <summary>
        /// Removes the events.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void RemoveEvents(TEntity entity)
        {
            var entityEventSourceWrapper = NHibernateHelper.CurrentSession.Get<EntityEventSourceWrapper>(entity.Id);
            if (entityEventSourceWrapper != null)
            {
                entityEventSourceWrapper.ClearEvents();
                NHibernateHelper.CurrentSession.Update(entityEventSourceWrapper);
                NHibernateHelper.CurrentSession.Flush();
            }
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        public void RemoveEvent(long id, IDomainEvent<TEntity> @event)
        {
            var entityEventSourceWrapper = NHibernateHelper.CurrentSession.Get<EntityEventSourceWrapper>(id);
            if (entityEventSourceWrapper != null)
            {
                entityEventSourceWrapper.RemoveEvent(@event.GetType().Name, @event.Version);
                NHibernateHelper.CurrentSession.Update(entityEventSourceWrapper);
                NHibernateHelper.CurrentSession.Flush();
            }
        }

        /// <summary>
        /// Finds the enity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        public EntityEventSource<TEntity, long> FindEntity(object spec)
        {
            throw new NotImplementedException();
        }

    }
}