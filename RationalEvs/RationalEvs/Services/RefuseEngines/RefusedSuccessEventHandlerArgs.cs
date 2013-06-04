namespace RationalEvs.Services.RefuseEngines
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class RefusedSuccessEventHandlerArgs<TEntity, TId> where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Gets or sets the aggregate root.
        /// </summary>
        /// <value>The aggregate root.</value>
        public IAggregateRoot<TEntity, TId> AggregateRoot { get; set; }
    }
}