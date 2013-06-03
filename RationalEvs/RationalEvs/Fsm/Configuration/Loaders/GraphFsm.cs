using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Core;

namespace RationalEvs.Fsm.Configuration.Loaders
{
    public class GraphFsm
    {
        public GraphFsm()
        {
            States = new Dictionary<int, State>();
            Transitions = new List<Transition>();
        }

        public IDictionary<int, State> States { get; private set; }

        public IList<Transition> Transitions { get; private set; }

        public ILog Logger { get; set; }

        /// <summary>
        ///   Adds the state.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <param name="state"> The state. </param>
        /// <param name="line"> </param>
        /// <param name="shape"> </param>
        /// <param name="nodosEstado"> </param>
        public void AddState(int id, string state, string line, string shape, int nodosEstado)
        {
            var s = new State { Id = id, Name = state, Line = line, Shape = shape };
            if (s.IsInternalState())
            {
                Logger.Debug(string.Format("Adding State {0}", state));
                States.Add(nodosEstado + id, new State { Id = nodosEstado + id, Name = string.Format("PendientePublicar {0}", state), Line = line, Shape = shape });

                Logger.Debug(string.Format("Adding State {0}", state));
                States.Add(id, s);
            }
            else
            {
                Logger.Debug(string.Format("Adding State {0}", state));
                States.Add(id, s);
            }
        }

        /// <summary>
        ///   Adds the transition.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="target"> The target. </param>
        /// <param name="domainEvent"> The domain event. </param>
        public void AddTransition(int source, int target, string domainEvent)
        {
            var eventData = domainEvent.Split(new[] { ";" }, 0);

            var isInternal = States.Single(x => x.Value.Name == States[target].Name).Value.IsInternalState();

            if (isInternal)
            {
                var xgmlTransition = new Transition
                {
                    Event = eventData[0],
                    Source = States[source].Name,
                    Target = string.Format("PendientePublicar {0}",States[target].Name),
                    Weight = eventData.Length > 1 ? int.Parse(eventData[1]) : 1000
                };

                Logger.Debug(string.Format("Adding Transition:  {0} -> {1} => {2}", xgmlTransition.Source, xgmlTransition.Target, xgmlTransition.Event));
                Transitions.Add(xgmlTransition);

                if (!Transitions.Any(x => x.Event == "InternalDomainEvent" && x.Source == string.Format("PendientePublicar {0}", States[target].Name)
                    && x.Target == States[target].Name))
                {
                    xgmlTransition = new Transition
                        {
                            Event = "InternalDomainEvent",
                            Source = string.Format("PendientePublicar {0}", States[target].Name),
                            Target = States[target].Name,
                            Weight = eventData.Length > 1 ? int.Parse(eventData[1]) : 1000
                        };

                    Logger.Debug(string.Format("Adding Transition:  {0} -> {1} => {2}",
                                                               xgmlTransition.Source, xgmlTransition.Target,
                                                               xgmlTransition.Event));
                    Transitions.Add(xgmlTransition);
                }
            }
            else
            {
                var xgmlTransition = new Transition
                    {
                        Event = eventData[0],
                        Source = States[source].Name,
                        Target = States[target].Name,
                        Weight = eventData.Length > 1 ? int.Parse(eventData[1]) : 1000
                    };

                Logger.Debug(string.Format("Adding Transition:  {0} -> {1} => {2}",
                                                           xgmlTransition.Source, xgmlTransition.Target,
                                                           xgmlTransition.Event));
                Transitions.Add(xgmlTransition);
            }
        }

        /// <summary>
        ///   Gets the states.
        /// </summary>
        /// <returns> </returns>
        public IEnumerable<string> GetStates()
        {
            return States.Values.Select(state => state.Name);
        }

        /// <summary>
        /// Gets the transitions.
        /// </summary>
        /// <param name="domainEventAssembly">The domain event assembly.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public IEnumerable<FsmTransition> GetTransitions(Assembly domainEventAssembly, string @namespace)
        {
            foreach (var transition in Transitions)
            {
                Type domainEvent =
                    transition.Event.Contains("InternalDomainEvent") ? Assembly.GetExecutingAssembly().GetType(string.Format("{0}.{1}", "Mrw.Infraestructura.Core.Domain.EventSourcing.Events", transition.Event)) 
                    : domainEventAssembly.GetType(string.Format("{0}.{1}", @namespace, transition.Event));

                if (domainEvent != null)
                {
                    yield return new FsmTransition
                    {
                        Description = transition.Event,
                        SourceState = transition.Source,
                        TargetState = transition.Target,
                        DomainEvent = domainEvent,
                        Weight = transition.Weight
                    };
                }
                else
                {
                    Logger.Warn(string.Format("Descartada Transicion:  {0} -> {1} NO EXISTE EL EVENTO {2}", transition.Source, transition.Target, transition.Event));
                }
            }
        }
    }
}