using System.Collections.Generic;

namespace RationalEvs.Fsm.Configuration
{
    public class FsmConfigurator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FsmConfigurator"/> class.
        /// </summary>
        /// <param name="startState">The start state.</param>
        /// <param name="transitions">The transitions.</param>
        /// <param name="states">The states.</param>
        public FsmConfigurator(string startState, IEnumerable<string> states, IEnumerable<FsmTransition> transitions)
        {
            States = states;
            Transitions = transitions;
            StartState = startState;
            FinalStates = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FsmConfigurator"/> class.
        /// </summary>
        /// <param name="startState">The start state.</param>
        /// <param name="states">The states.</param>
        /// <param name="transitions">The transitions.</param>
        /// <param name="finalStates">The final states.</param>
        public FsmConfigurator(string startState, IEnumerable<string> states, IEnumerable<FsmTransition> transitions, IEnumerable<string> finalStates)
        {
            States = states;
            Transitions = transitions;
            FinalStates = finalStates;
            StartState = startState;
        }

        public IEnumerable<string> States { get; private set; }

        /// <summary>
        /// Gets or sets the final states.
        /// </summary>
        /// <value>The final states.</value>
        public IEnumerable<string> FinalStates { get; private set; }

        /// <summary>
        /// Gets the start state.
        /// </summary>
        /// <value>The start state.</value>
        public string StartState { get; private set; }

        /// <summary>
        /// Gets the transitions.
        /// </summary>
        /// <value>The transitions.</value>
        public IEnumerable<FsmTransition> Transitions { get; private set; }
    }
}