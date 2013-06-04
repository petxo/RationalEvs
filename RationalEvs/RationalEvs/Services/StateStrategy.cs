using System;
using System.Collections.Generic;
using RationalEvs.Events;
using RationalEvs.Factories;
using RationalEvs.Services.RefuseEngines;
using RationalEvs.Services.RefuseEngines.Factories;

namespace RationalEvs.Services
{
    public abstract class StateStrategy<TEntity, TId> : IStateStrategy<TEntity, TId> 
        where TEntity : IVersionableEntity<TId>, new()
    {

        private const int MaxEvents = 50;
        private readonly IRefuseEngine<TEntity, TId> _refuseEngine;
        private readonly int _maxEvents;
        private readonly IDictionary<string, Action<AggregateRoot<TEntity, TId>>> _strategies;
        private readonly IAggregateFactory<TEntity, TId> _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStrategy&lt;TEntity, TId&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected StateStrategy(IAggregateFactory<TEntity, TId> factory)
            : this(factory, (IRefuseEngine<TEntity, TId>)RefuseEngineFactory.DefaultRefuseEngine<TEntity, TId>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStrategy&lt;TEntity, TId&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="maxEvents">The max events.</param>
        protected StateStrategy(IAggregateFactory<TEntity, TId> factory, int maxEvents)
            : this(factory, RefuseEngineFactory.DefaultRefuseEngine<TEntity, TId>(), maxEvents)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateStrategy&lt;TEntity, TId&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="refuseEngine">The refuse engine.</param>
        protected StateStrategy(IAggregateFactory<TEntity, TId> factory, IRefuseEngine<TEntity, TId> refuseEngine)
            : this(factory, refuseEngine, MaxEvents)
        {
            _refuseEngine = refuseEngine;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateStrategy&lt;TEntity, TId&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="refuseEngine">The refuse engine.</param>
        /// <param name="maxEvents">The max events.</param>
        protected StateStrategy(IAggregateFactory<TEntity, TId> factory, IRefuseEngine<TEntity, TId> refuseEngine, int maxEvents)
        {
            _refuseEngine = refuseEngine;
            _maxEvents = maxEvents;
            _factory = factory;
            _strategies = new Dictionary<string, Action<AggregateRoot<TEntity, TId>>>();
            ConfigureStrategy();
        }

        /// <summary>
        /// Gets the strategies.
        /// </summary>
        /// <value>The strategies.</value>
        protected IDictionary<string, Action<AggregateRoot<TEntity, TId>>> Strategies
        {
            get { return _strategies; }
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>The factory.</value>
        protected IAggregateFactory<TEntity, TId> Factory
        {
            get { return _factory; }
        }

        public virtual event ActionStateNotificationHandler<TEntity> ActionStateNotification;

        public virtual event EnityNotExistNotificationHandler<TId> EnityNotExistNotification;

        protected void OnEnityNotExistNotification(TId id)
        {
            var handler = EnityNotExistNotification;
            if (handler != null) handler(this, new EnityNotExistNotificationHandlerArgs<TId> { Id = id });
        }

        /// <summary>
        ///   Called when [action state notification].
        /// </summary>
        /// <param name="state"> The state. </param>
        /// <param name="entity"> The entity. </param>
        protected void OnActionStateNotification(string state, TEntity entity)
        {
            var handler = ActionStateNotification;
            if (handler != null)
                handler(this, new ActionStateNotificationArgs<TEntity> { State = state, Entity = entity });
        }

        protected abstract void ConfigureStrategy();

        protected void AddAction(string state, Action<AggregateRoot<TEntity, TId>> action)
        {
            _strategies.Add(state, action);
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The id.</param>
        public virtual void GetEntity(TId id)
        {
            var aggregateRoot = _factory.GetById(id);
            if (aggregateRoot != null)
                OnActionStateNotification(aggregateRoot.State, aggregateRoot.Root);
            else
                OnEnityNotExistNotification(id);
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root.</param>
        protected void DoAction(AggregateRoot<TEntity, TId> aggregateRoot)
        {
            if ((!string.IsNullOrEmpty(aggregateRoot.State)) && (_strategies.ContainsKey(aggregateRoot.State)))
            {
                _strategies[aggregateRoot.State].Invoke(aggregateRoot);
            }
        }


        /// <summary>
        /// Realiza tareas definidas por la aplicación asociadas a la liberación o al restablecimiento de recursos no administrados.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec"> </param>
        /// <returns></returns>
        public TEntity FindEntity(object spec)
        {
            var aggregateRoot = Factory.FindEntity(spec);
            var findEntity = aggregateRoot != null ? aggregateRoot.Root : default(TEntity);
            return findEntity;
        }

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="domainEvent">The domain event.</param>
        public void ApplyEvent(TId id, IDomainEvent<TEntity> domainEvent)
        {
            AggregateRoot<TEntity, TId> aggregateRoot = null;
            try
            {
                aggregateRoot = AddEvent(id, domainEvent);
                aggregateRoot.SaveHistory();
                DoAction(aggregateRoot);
            }
            finally
            {
                if (aggregateRoot != null)
                {
                    if (_maxEvents > 0 && aggregateRoot.AppliedEvents.Count > _maxEvents)
                        aggregateRoot.CreateSnapShot();

                    aggregateRoot.ReleaseEntity();
                }
            }
        }

        public void ApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> domainEvents)
        {
            AggregateRoot<TEntity, TId> aggregateRoot = null;
            try
            {
                aggregateRoot = AddEvents(id, domainEvents);
                aggregateRoot.SaveHistory();
                DoAction(aggregateRoot);
            }
            finally
            {
                if (aggregateRoot != null)
                {
                    if (_maxEvents > 0 && aggregateRoot.AppliedEvents.Count > _maxEvents)
                        aggregateRoot.CreateSnapShot();

                    aggregateRoot.ReleaseEntity();
                }
            }
        }

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="domainEvent">The domain event.</param>
        public AggregateRoot<TEntity, TId> AddEvent(TId id, IDomainEvent<TEntity> domainEvent)
        {
            return Factory.GetAndApplyEvent(id, domainEvent, (root, @event) => RefuseEngine.Refuse(root, @event));
        }

        /// <summary>
        /// Adds the events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="domainEvents">The domain events.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> AddEvents(TId id, IEnumerable<IDomainEvent<TEntity>> domainEvents)
        {
            return Factory.GetAndApplyEvents(id, domainEvents, (root, @event) => RefuseEngine.Refuse(root, @event));
        }


        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetIntermediateEvents(TId id, string finalState)
        {
            var aggregateRoot = Factory.GetById(id);
            return aggregateRoot != null ? aggregateRoot.GetIntermediateEvents(finalState) : null;
        }

        /// <summary>
        /// Gets or sets the refuse engine.
        /// </summary>
        /// <value>The refuse engine.</value>
        public IRefuseEngine<TEntity, TId> RefuseEngine
        {
            get { return _refuseEngine; }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}