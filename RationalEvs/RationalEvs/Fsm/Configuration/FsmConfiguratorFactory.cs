using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RationalEvs.Fsm.Configuration.Loaders;
using RationalEvs.Fsm.Configuration.Loaders.Xgml;

namespace RationalEvs.Fsm.Configuration
{
    public static class FsmConfiguratorFactory
    {

        public static FsmXmglConfigurator WithXgml(string filename)
        {
            return new FsmXmglConfigurator(filename);
        }

        public class FsmXmglConfigurator
        {
            private GraphFsm _graphFsm;
            private Assembly _assembly;
            private string _doaminEventNamespace;
            private string _startState;
            private IEnumerable<string> _finalStates = new List<string>();

            public FsmXmglConfigurator(string filename)
            {
                var xgmlLoader = new XgmlLoader();
                _graphFsm = xgmlLoader.LoadFrom(filename);
                
                var startState = _graphFsm.States.Values.FirstOrDefault(s => s.IsStartState());
                if (startState != null)
                    _startState = startState.Name;

                _finalStates = _graphFsm.States.Values.Where(s => s.IsStartState()).Select(s => s.Name).ToList();
            }

            public FsmXmglConfigurator WithDomainAssembly(Assembly assembly, string doaminEventNamespace)
            {
                _assembly = assembly;
                _doaminEventNamespace = doaminEventNamespace;
                return this;
            }

            public FsmXmglConfigurator WithDomainAssembly<T>(string doaminEventNamespace)
            {
                _assembly = Assembly.GetAssembly(typeof(T));
                _doaminEventNamespace = doaminEventNamespace;
                return this;
            }


            public FsmXmglConfigurator WithDomainAssembly(Type type, string doaminEventNamespace)
            {
                _assembly = Assembly.GetAssembly(type);
                _doaminEventNamespace = doaminEventNamespace;
                return this;
            }


            public FsmXmglConfigurator SetInitialState(string startState)
            {
                _startState = startState;
                return this;
            }


            public FsmXmglConfigurator SetFinalStates(IEnumerable<string> finalStates)
            {
                _finalStates = finalStates;
                return this;
            }

            public FsmConfigurator Create()
            {
                return new FsmConfigurator(_startState, _graphFsm.GetStates(), _graphFsm.GetTransitions(_assembly, _doaminEventNamespace), _finalStates);
            }
        }
    }
}