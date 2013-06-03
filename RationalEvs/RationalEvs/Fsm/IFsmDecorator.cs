using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Jolt.Automata;
using RationalEvs.Events;
using QuickGraph;

namespace RationalEvs.Fsm
{
    public interface IFsmDecorator<TEntity>
    {
        /// <summary>
        /// Occurs when [on transition domain event].
        /// </summary>
        event TransitionDomainEventHandler<TEntity> OnTransitionDomainEvent;

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        TEntity Root { get; }

         /// <summary>
        /// Gets the get graph.
        /// </summary>
        /// <value>The get graph.</value>
        IBidirectionalGraph<string, Transition<IDomainEvent<TEntity>>> GetGraph { get; }

        /// <summary>
        /// Consumes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        ConsumptionResult<IDomainEvent<TEntity>> Consume(IEnumerable<IDomainEvent<TEntity>> events);

        /// <summary>
        /// Toes the graph viz.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void ToGraphViz(TextWriter writer);

        /// <summary>
        /// Toes the graph viz.
        /// </summary>
        /// <param name="filename">The filename.</param>
        void ToGraphViz(string filename);

        /// <summary>
        /// Toes the graph ML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void ToGraphML(XmlWriter writer);

        /// <summary>
        /// Toes the graph ML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        void ToGraphML(string filename);

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="finalState">The final state.</param>
        IEnumerable<Type> GetIntermediateEvents(IEnumerable<IDomainEvent<TEntity>> events, string finalState);

        /// <summary>
        /// Gets the intermediate events.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="finalState">The final state.</param>
        /// <returns></returns>
        IEnumerable<Type> GetIntermediateEvents(string initialState, string finalState);
    }
}