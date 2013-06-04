namespace RationalEvs.Services
{
    public class ActionStateNotificationArgs<TEntity>
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public TEntity Entity { get; set; }
    }
}