using System;
using System.Collections.Generic;
using RationalEvs.Events;

namespace RationalEvs.Services
{
    public class EventSourcingService<TEntity, TId> : IEventSourcingService<TEntity, TId>
        where TEntity : IVersionableEntity<TId>, new()
    {
        private readonly IStateStrategy<TEntity, TId> _stateStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourcingService&lt;TEntity, TId&gt;"/> class.
        /// </summary>
        /// <param name="stateStrategy">The state strategy.</param>
        public EventSourcingService(IStateStrategy<TEntity, TId> stateStrategy)
        {
            _stateStrategy = stateStrategy;
            _stateStrategy.ActionStateNotification += OnActionStateNotificationHandler;
            _stateStrategy.EnityNotExistNotification += OnEnityNotExistNotificationHandler;
        }

        private void OnEnityNotExistNotificationHandler(object sender, EnityNotExistNotificationHandlerArgs<TId> args)
        {
            OnEnityNotExistNotification(args);
        }

        private void OnActionStateNotificationHandler(object sender, ActionStateNotificationArgs<TEntity> args)
        {
            OnActionStateNotification(args);
        }

        /// <summary>
        ///   Occurs when [action state notification].
        /// </summary>
        public virtual event ActionStateNotificationHandler<TEntity> ActionStateNotification;

        public virtual event EnityNotExistNotificationHandler<TId> EnityNotExistNotification;

        protected void OnEnityNotExistNotification(EnityNotExistNotificationHandlerArgs<TId> args)
        {
            EnityNotExistNotificationHandler<TId> handler = EnityNotExistNotification;
            if (handler != null) handler(this, args);
        }

        /// <summary>
        /// Gets the state strategy.
        /// </summary>
        /// <value>The state strategy.</value>
        protected IStateStrategy<TEntity, TId> StateStrategy
        {
            get { return _stateStrategy; }
        }

        private void OnActionStateNotification(ActionStateNotificationArgs<TEntity> args)
        {
            ActionStateNotificationHandler<TEntity> handler = ActionStateNotification;
            if (handler != null) handler(this, args);
        }

        /// <summary>
        ///   Adds the event.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <param name="domainEvent"> The domain event. </param>
        public void AddEvent(TId id, IDomainEvent<TEntity> domainEvent)
        {
            _stateStrategy.ApplyEvent(id, domainEvent);
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The id.</param>
        public void GetEntity(TId id)
        {
            _stateStrategy.GetEntity(id);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"> <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stateStrategy.ActionStateNotification -= OnActionStateNotificationHandler;
                _stateStrategy.EnityNotExistNotification -= OnEnityNotExistNotificationHandler;
            }
        }

        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public TEntity FindEntity(object spec)
        {
            return StateStrategy.FindEntity(spec);
        }

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetIntermediateEvents(TId id, string finalState)
        {
            return StateStrategy.GetIntermediateEvents(id, finalState);
        }
    }
}