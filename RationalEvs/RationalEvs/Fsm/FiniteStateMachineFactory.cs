using Jolt.Automata;
using RationalEvs.Events;
using RationalEvs.Fsm.Configuration;

namespace RationalEvs.Fsm
{
    public static class FiniteStateMachineFactory
    {
        /// <summary>
        /// Creates the specified state machine configurator.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <param name="fsmConfigurator">The FSM configurator.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="startState">The start state.</param>
        /// <returns></returns>
        public static FsmDecorator<TEntity, TId> Create<TEntity, TId>(FsmConfigurator fsmConfigurator, TEntity entity, string startState)
            where TEntity : IVersionableEntity<TId>
        {
            var finiteStateMachine = new FiniteStateMachine<IDomainEvent<TEntity>>();
            var fsmDecorator = new FsmDecorator<TEntity, TId>(finiteStateMachine, entity);

            fsmDecorator.AddStates(fsmConfigurator.States);
            fsmDecorator.StartState = string.IsNullOrEmpty(startState) ? fsmConfigurator.StartState : startState;
            fsmDecorator.AddTransitions(fsmConfigurator.Transitions);
            fsmDecorator.SetFinalStates(fsmConfigurator.FinalStates);

            return fsmDecorator;
        }
    }
}