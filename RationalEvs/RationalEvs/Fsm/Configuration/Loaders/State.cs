namespace RationalEvs.Fsm.Configuration.Loaders
{
    public class State
    {
        private const string LineStyleStart = "dashed";
        private const string LineStyleFinal = "dashed_dotted";
        private const string ShapeStyle = "ellipse";
        private const string ShapeInternalStyle = "octagon";

        public int Id { get; set; }

        public string Name { get; set; }

        public string Line { get; set; }

        public string Shape { get; set; }

        /// <summary>
        /// Determines whether [is start state].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is start state]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStartState()
        {
            return Line == LineStyleStart;
        }

        /// <summary>
        /// Determines whether [is final state].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is final state]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFinalState()
        {
            return Line == LineStyleFinal;
        }

        /// <summary>
        /// Determines whether [is internal state].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is internal state]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInternalState()
        {
            return Shape == ShapeInternalStyle;
        }
    }
}