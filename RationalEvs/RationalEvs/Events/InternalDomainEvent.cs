namespace RationalEvs.Events
{
    public class InternalDomainEvent: IDomainEvent<object>
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public long Version { get; set; }

        /// <summary>
        /// Applies the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        public void Apply(object root)
        {
            
        }

        /// <summary>
        /// Determines whether this instance can apply the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>
        ///   <c>true</c> if this instance can apply the specified root; otherwise, <c>false</c>.
        /// </returns>
        public bool CanApply(object root)
        {
            return true;
        }
    }
}