namespace RationalEvs.Events
{
    public interface IDomainEvent<in TEntity>
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        long Version { get; set; }

        /// <summary>
        /// Applies the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        void Apply(TEntity root);

        /// <summary>
        /// Determines whether this instance can apply the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can apply the specified root; otherwise, <c>false</c>.
        /// </returns>
        bool CanApply(TEntity root);
    }
}