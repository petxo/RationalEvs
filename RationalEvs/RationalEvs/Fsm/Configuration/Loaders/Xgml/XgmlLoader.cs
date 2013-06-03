using System.IO;
using System.Xml.XPath;

namespace RationalEvs.Fsm.Configuration.Loaders.Xgml
{
    public class XgmlLoader : IFsmLoader
    {

        public GraphFsm LoadFrom(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            var document = new XPathDocument(stream);
            var navigator = document.CreateNavigator();

            var xgmlFsm = new GraphFsm();

            ReadStates(xgmlFsm, navigator);
            ReadTransitions(xgmlFsm, navigator);

            return xgmlFsm;
        }

        private static void ReadStates(GraphFsm graphFsm, XPathNavigator navigator)
        {
            var nodosEstado = navigator.Select("section[@name='xgml']/section[@name='graph']/section[@name='node']");
            foreach (XPathNavigator nodo in nodosEstado)
            {
                var state = string.Empty;
                var id = 0;
                var line = string.Empty;
                var shape = string.Empty;

                var value = nodo.Select("attribute[@key='label']");
                if (value.MoveNext())
                    state = value.Current.Value;

                value = nodo.Select("attribute[@key='id']");
                if (value.MoveNext())
                    id = value.Current.ValueAsInt;

                value = nodo.Select("section[@name='graphics']/attribute[@key='outlineStyle']");
                if (value.MoveNext())
                    line = value.Current.Value;

                value = nodo.Select("section[@name='graphics']/attribute[@key='type']");
                if (value.MoveNext())
                    shape = value.Current.Value;

                graphFsm.AddState(id, state, line, shape, nodosEstado.Count);
            }
        }

        private static void ReadTransitions(GraphFsm graphFsm, XPathNavigator navigator)
        {
            var nodosEstado = navigator.Select("section[@name='xgml']/section[@name='graph']/section[@name='edge']");
            foreach (XPathNavigator nodo in nodosEstado)
            {
                var domainEvent = string.Empty;
                var source = 0;
                var target = 0;

                var value = nodo.Select("attribute[@key='label']");
                if (value.MoveNext())
                    domainEvent = value.Current.Value;

                value = nodo.Select("attribute[@key='source']");
                if (value.MoveNext())
                    source = value.Current.ValueAsInt;

                value = nodo.Select("attribute[@key='target']");
                if (value.MoveNext())
                    target = value.Current.ValueAsInt;
                
                if (!string.IsNullOrEmpty(domainEvent)) //Hay transisiones sin eventos
                    graphFsm.AddTransition(source, target, domainEvent);
            }
        }
    }
}