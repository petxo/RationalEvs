using System.Collections.Generic;
using System;
using System.Linq;
using RationalEvs.Appliers;
using RationalEvs.Events;
using RationalEvs.Fsm;
using RationalEvs.Repositories;
using log4net;
using log4net.Core;

namespace RationalEvs
{
    public class AggregateRoot<TEntity, TId> : IAggregateRoot<TEntity, TId>
        where TEntity : IVersionableEntity<TId>, new()
    {
        private readonly IEventsRepository<TEntity, TId> _repository;
        private readonly List<IDomainEvent<TEntity>> _appliedEvents;
        private readonly IFsmDecorator<TEntity> _stateMachineDecorator;
        private readonly IApplierEvents _applierEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="stateMachineDecorator">The state machine decorator.</param>
        /// <param name="applierEvents">The applier events.</param>
        public AggregateRoot(IEventsRepository<TEntity, TId> repository, IFsmDecorator<TEntity> stateMachineDecorator, IApplierEvents applierEvents)
        {
            _repository = repository;
            _stateMachineDecorator = stateMachineDecorator;
            _applierEvents = applierEvents;
            _appliedEvents = new List<IDomainEvent<TEntity>>();
        }

        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public TEntity Root
        {
            get { return _stateMachineDecorator.Root; }
        }

        /// <summary>
        /// Gets the applied events.
        /// </summary>
        /// <value>The applied events.</value>
        public IList<IDomainEvent<TEntity>> AppliedEvents { get { return _appliedEvents; } }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; private set; }


        /// <summary>
        /// Restores the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        public void ApplyEvents(IEnumerable<IDomainEvent<TEntity>> events)
        {
            _applierEvents.GetOrderedEvents(events, Root.Version).ToList()
                .ForEach(x => Logger.Debug(string.Format("APLICANDO EVENTO: {0}-{1}", x.GetType().Name, x.Version)));

            _appliedEvents.AddRange(_applierEvents.GetOrderedEvents(events, Root.Version));

            Regenerate();
        }

        /// <summary>
        /// Regenerates this instance.
        /// </summary>
        public void Regenerate()
        {
            var consumptionResult = _stateMachineDecorator.Consume(_applierEvents.GetOrderedEvents(_appliedEvents, Root.Version));

            if (consumptionResult.LastState.Equals("Jolt.FSM.ImplicitErrorState"))
            {
                LastEventApplied = consumptionResult.LastSymbol;
                InvokeDomainEventErroneo(consumptionResult.LastSymbol);
            }
            else
            {
                State = consumptionResult.LastState;
            }
        }

        /// <summary>
        /// Gets or sets the last event applied.
        /// </summary>
        /// <value>
        /// The last event applied.
        /// </value>
        protected IDomainEvent<TEntity> LastEventApplied { get; set; }

        /// <summary>
        /// Restores the specified events.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void ApplyEvent(IDomainEvent<TEntity> @event)
        {
            _appliedEvents.Add(@event);
            Regenerate();
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void RemoveEvent(IDomainEvent<TEntity> @event)
        {
            _repository.RemoveEvent(Root.Id, @event);
            RemoveFromAppliedEvents(@event);
        }

        /// <summary>
        /// Removes from applied events.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void RemoveFromAppliedEvents(IDomainEvent<TEntity> @event)
        {
            _appliedEvents.Remove(@event);
        }

        /// <summary>
        /// Creates the snap shot.
        /// </summary>
        public void CreateSnapShot()
        {
            _repository.SaveSnapShot(Root, _applierEvents.TypeSnapShot, _appliedEvents, State);
        }

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetIntermediateEvents(string finalState)
        {
            return State != finalState? _stateMachineDecorator.GetIntermediateEvents(State, finalState) : new List<Type>();
        }

        /// <summary>
        /// Occurs when [domain event erroneo].
        /// </summary>
        public event DomainEventErroneoEventHandler<TEntity> DomainEventErroneo;

        /// <summary>
        /// Invokes the domain event erroneo.
        /// </summary>
        /// <param name="event">The @event.</param>
        private void InvokeDomainEventErroneo(IDomainEvent<TEntity> @event)
        {
            var handler = DomainEventErroneo;
            if (handler != null) handler(this, new DomainEventErroneoEventHandlerArgs<TEntity>() { Event = @event });
        }

        /// <summary>
        /// Releases the entity.
        /// </summary>
        public void ReleaseEntity()
        {
            _repository.ReleaseEntity(Root.Id);
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        public void SaveHistory()
        {
            _repository.SaveHistory(Root, State, LastEventApplied, _appliedEvents, DateTime.UtcNow, Root.Version);
        }
    }
}