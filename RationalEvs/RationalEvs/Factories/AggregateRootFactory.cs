using System.Collections.Generic;
using System;
using RationalEvs.Appliers;
using RationalEvs.Entities;
using RationalEvs.Events;
using RationalEvs.Fsm;
using RationalEvs.Fsm.Configuration;
using RationalEvs.Repositories;
using log4net;
using log4net.Core;

namespace RationalEvs.Factories
{
    public class AggregateRootFactory<TEntity, TId> : IAggregateFactory<TEntity, TId>
        where TEntity : class, IVersionableEntity<TId>, new()
    {
        private readonly IApplierEvents _applierEvents;

        private readonly IEventsRepository<TEntity, TId> _repository;
        private readonly FsmConfigurator _fsmConfigurator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootFactory&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="fsmConfigurator">The FSM configurator.</param>
        public AggregateRootFactory(IEventsRepository<TEntity, TId> repository, FsmConfigurator fsmConfigurator)
        {
            _repository = repository;
            _fsmConfigurator = fsmConfigurator;
            _applierEvents = ApplierEventFactory.DefaultApplier();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootFactory&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="fsmConfigurator">The FSM configurator.</param>
        /// <param name="applierEvents">The applier events.</param>
        public AggregateRootFactory(IEventsRepository<TEntity, TId> repository, FsmConfigurator fsmConfigurator, IApplierEvents applierEvents)
        {
            _repository = repository;
            _fsmConfigurator = fsmConfigurator;
            _applierEvents = applierEvents;
        }

        public ILog Logger { get; set; }

        /// <summary>
        /// Creates the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> GetById(TId id)
        {
            var entityEventSource = _repository.GetEvents(id);
            return entityEventSource != null ? CreateAggregateRoot(id, entityEventSource) : null;
        }

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> GetAndApplyEvent(TId id, IDomainEvent<TEntity> @event)
        {
            var entityEventSource = _repository.AddEvent(id, @event);
            return CreateAggregateRoot(id, entityEventSource);
        }

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The @event.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> GetAndApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> events)
        {
            EntityEventSource<TEntity, TId> entityEventSource = null;
            foreach (var domainEvent in events)
            {
                entityEventSource = _repository.AddEvent(id, domainEvent);
            }
            
            return CreateAggregateRoot(id, entityEventSource);
        }

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="event">The @event.</param>
        /// <param name="domainEventErroneoAction">The domain event erroneo action.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> GetAndApplyEvent(TId id, IDomainEvent<TEntity> @event, Action<AggregateRoot<TEntity, TId>, IDomainEvent<TEntity>> domainEventErroneoAction)
        {
            var entityEventSource = _repository.AddEvent(id, @event);
            return CreateAggregateRoot(id, entityEventSource, domainEventErroneoAction);
        }

        /// <summary>
        /// Gets the and apply event.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="events">The @event.</param>
        /// <param name="domainEventErroneoAction">The domain event erroneo action.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> GetAndApplyEvents(TId id, IEnumerable<IDomainEvent<TEntity>> events, Action<AggregateRoot<TEntity, TId>, IDomainEvent<TEntity>> domainEventErroneoAction)
        {
            EntityEventSource<TEntity, TId> entityEventSource = null;
            foreach (var domainEvent in events)
            {
                entityEventSource = _repository.AddEvent(id, domainEvent);
            }
            return CreateAggregateRoot(id, entityEventSource, domainEventErroneoAction);
        }

        /// <summary>
        /// Creates the aggregate root.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityEventSource">The entity event source.</param>
        /// <param name="domainEventErroneoAction">The domainEventErroneoAction.</param>
        /// <returns></returns>
        private AggregateRoot<TEntity, TId> CreateAggregateRoot(TId id, EntityEventSource<TEntity, TId> entityEventSource, Action<AggregateRoot<TEntity, TId>, IDomainEvent<TEntity>> domainEventErroneoAction = null)
        {
            Logger.Debug(string.Format("Creando la entidad {1} {0}", id, typeof(TEntity).Name));

            var root = entityEventSource.SnapShot ?? new TEntity { Id = id, Version = 0 };

            var fsmDecorator = FiniteStateMachineFactory.Create<TEntity, TId>(_fsmConfigurator, root, entityEventSource.State);
            var aggregate = new AggregateRoot<TEntity, TId>(_repository, fsmDecorator, _applierEvents) { Logger = Logger };

            aggregate.DomainEventErroneo += (sender, args) =>
                {
                    if (domainEventErroneoAction != null)
                    {
                        domainEventErroneoAction.Invoke((AggregateRoot<TEntity, TId>)sender, args.Event);
                    }
                };

            Logger.Debug(string.Format("Reconstruyendo la entidad {0}", id));
            aggregate.ApplyEvents(entityEventSource.Events);

            Logger.Debug(string.Format("Estado de la entidad {0}: {1}", id, aggregate.State));

            return aggregate;
        }


        /// <summary>
        /// Finds the entity.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public AggregateRoot<TEntity, TId> FindEntity(object spec)
        {
            var entityEventSource = _repository.FindEntity(spec);
            return entityEventSource != null ? CreateAggregateRoot(entityEventSource.Id, entityEventSource) : null;
        }
    }
}
