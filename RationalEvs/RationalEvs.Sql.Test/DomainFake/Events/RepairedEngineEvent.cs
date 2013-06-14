using RationalEvs.Events;

namespace RationalEvs.Sql.Test.DomainFake.Events
{
    public class RepairedEngineEvent : IDomainEvent<Ship>
    {
        public void Apply(Ship root)
        {
            
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        /// Determines whether this instance can apply the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can apply the specified root; otherwise, <c>false</c>.
        /// </returns>
        public bool CanApply(Ship root)
        {
            return true;
        }
    }
}