using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Jolt.Automata;
using RationalEvs.Events;
using RationalEvs.Fsm.Configuration;
using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;

namespace RationalEvs.Fsm
{
    public delegate void TransitionDomainEventHandler<TEntity>(object sender, TransitionDomainEventArgs<TEntity> args);

    public class TransitionDomainEventArgs<TEntity>
    {
        public IDomainEvent<TEntity> DomainEvent { get; set; }
    }

    public class FsmDecorator<TEntity, TId> : IFsmDecorator<TEntity>
        where TEntity : IVersionableEntity<TId>
    {
        private readonly FiniteStateMachine<IDomainEvent<TEntity>> _finiteStateMachine;

        private readonly IDictionary<Transition<IDomainEvent<TEntity>>, FsmTransition> _fsmTransitions;

        internal FsmDecorator(FiniteStateMachine<IDomainEvent<TEntity>> finiteStateMachine, TEntity root)
        {
            _fsmTransitions = new Dictionary<Transition<IDomainEvent<TEntity>>, FsmTransition>();
            _finiteStateMachine = finiteStateMachine;
            Root = root;
        }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public TEntity Root { get; private set; }

        /// <summary>
        /// Occurs when [on transition domain event].
        /// </summary>
        public event TransitionDomainEventHandler<TEntity> OnTransitionDomainEvent;

        /// <summary>
        /// Called when [on transition domain event].
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void InvokeOnTransitionDomainEvent(IDomainEvent<TEntity> domainEvent)
        {
            var handler = OnTransitionDomainEvent;
            if (handler != null) handler(this, new TransitionDomainEventArgs<TEntity>() { DomainEvent = domainEvent });
        }

        /// <summary>
        /// Gets the get graph.
        /// </summary>
        /// <value>The get graph.</value>
        public IBidirectionalGraph<string, Transition<IDomainEvent<TEntity>>> GetGraph
        {
            get
            {
                return _finiteStateMachine.AsGraph;
            }
        }

        /// <summary>
        /// Gets or sets the start state.
        /// </summary>
        /// <value>The start state.</value>
        internal string StartState
        {
            get
            {
                return _finiteStateMachine.StartState;
            }
            set
            {
                _finiteStateMachine.StartState = value;
            }
        }

        /// <summary>
        /// Sets the final state.
        /// </summary>
        /// <param name="finalState">The final state.</param>
        internal void SetFinalState(string finalState)
        {
            _finiteStateMachine.SetFinalState(finalState);
        }


        /// <summary>
        /// Sets the final states.
        /// </summary>
        /// <param name="finalStates">The final states.</param>
        internal void SetFinalStates(IEnumerable<string> finalStates)
        {
            _finiteStateMachine.SetFinalStates(finalStates);
        }

        /// <summary>
        /// Adds the states.
        /// </summary>
        /// <param name="states">The states.</param>
        internal void AddStates(IEnumerable<string> states)
        {
            _finiteStateMachine.AddStates(states);
        }

        /// <summary>
        /// Adds the state.
        /// </summary>
        /// <param name="state">The state.</param>
        internal void AddState(string state)
        {
            _finiteStateMachine.AddState(state);
        }

        /// <summary>
        /// Adds the transitions.
        /// </summary>
        /// <param name="fsmTransitions">The FSM transitions.</param>
        internal void AddTransitions(IEnumerable<FsmTransition> fsmTransitions)
        {
            foreach (var fsmTransition in fsmTransitions)
            {
                AddTransition(fsmTransition);
            }
        }

        /// <summary>
        /// Adds the transition.
        /// </summary>
        /// <param name="fsmTransition">The FSM transition.</param>
        internal void AddTransition(FsmTransition fsmTransition)
        {
            var transition = new Transition<IDomainEvent<TEntity>>(fsmTransition.SourceState, fsmTransition.TargetState,
                @event => @event.GetType() == fsmTransition.DomainEvent && @event.CanApply(Root));

            transition.OnTransition += InvokeOnTransition;
            transition.Description = fsmTransition.Weight.ToString(CultureInfo.InvariantCulture);

            _finiteStateMachine.AddTransition(transition);
            _fsmTransitions.Add(transition, fsmTransition);
        }

        private void InvokeOnTransition(object sender, StateTransitionEventArgs<IDomainEvent<TEntity>> args)
        {
            args.InputSymbol.Apply(Root);
            Root.Version = Math.Max(Root.Version, args.InputSymbol.Version);
            InvokeOnTransitionDomainEvent(args.InputSymbol);
        }

        /// <summary>
        /// Consumes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        public ConsumptionResult<IDomainEvent<TEntity>> Consume(IEnumerable<IDomainEvent<TEntity>> events)
        {
            return _finiteStateMachine.Consume(@events);
        }

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="finalState">The final state.</param>
        public IEnumerable<Type> GetIntermediateEvents(IEnumerable<IDomainEvent<TEntity>> events, string finalState)
        {
            var fwAlgorithm = new FloydWarshallAllShortestPathAlgorithm<string, Transition<IDomainEvent<TEntity>>>(GetGraph, transition => _fsmTransitions[transition].Weight);
            fwAlgorithm.Compute();


            var consumptionResult = Consume(events);
            IEnumerable<Transition<IDomainEvent<TEntity>>> transitions;
            fwAlgorithm.TryGetPath(consumptionResult.LastState, finalState, out transitions);

            return transitions.Select(transition => _fsmTransitions[transition].DomainEvent).ToList();
        }

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetIntermediateEvents(string initialState, string finalState)
        {
            var fwAlgorithm = new FloydWarshallAllShortestPathAlgorithm<string, Transition<IDomainEvent<TEntity>>>(GetGraph, transition => _fsmTransitions[transition].Weight);
            fwAlgorithm.Compute();

            IEnumerable<Transition<IDomainEvent<TEntity>>> transitions;
            fwAlgorithm.TryGetPath(initialState, finalState, out transitions);

            return transitions.Select(transition => _fsmTransitions[transition].DomainEvent).ToList();
        }


        /// <summary>
        /// Toes the graph viz.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void ToGraphViz(TextWriter writer)
        {
            FsmConverter.ToGraphViz(_finiteStateMachine, writer);
        }

        /// <summary>
        /// Toes the graph viz.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ToGraphViz(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                FsmConverter.ToGraphViz(_finiteStateMachine, writer);
            }
        }

        /// <summary>
        /// Toes the graph ML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void ToGraphML(XmlWriter writer)
        {
            FsmConverter.ToGraphML(_finiteStateMachine, writer);
        }

        /// <summary>
        /// Toes the graph ML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ToGraphML(string filename)
        {
            using (var writer = XmlWriter.Create(filename))
            {
                FsmConverter.ToGraphML(_finiteStateMachine, writer);
            }
        }


    }
}