namespace RationalEvs
{
    public interface IRoot<TEntity, TId> where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        TEntity Root { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        string State { get; }

        /// <summary>
        /// Releases the entity.
        /// </summary>
        void ReleaseEntity();
    }
}