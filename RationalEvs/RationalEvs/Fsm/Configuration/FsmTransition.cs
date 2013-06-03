using System;

namespace RationalEvs.Fsm.Configuration
{
    public class FsmTransition
    {
        public string SourceState { get; set; }

        public string TargetState { get; set; }

        public string Description { get; set; }
        
        public Type DomainEvent { get; set; }

        public int Weight { get; set; }
    }
}