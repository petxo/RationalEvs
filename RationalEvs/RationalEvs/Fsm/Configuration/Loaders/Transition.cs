namespace RationalEvs.Fsm.Configuration.Loaders
{
    public class Transition
    {
        public string Event { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public int Weight { get; set; }
    }
}