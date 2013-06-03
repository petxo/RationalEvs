namespace RationalEvs.Events
{
    public interface IIdentificableDomainEvent<in TEntity, TId> : IDomainEvent<TEntity>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        TId Id { get; set; }
    }
}