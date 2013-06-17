namespace RationalEvs.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IQuerySnapshotBuilder<TQuery, TEntity>
    {

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <param name="snapShotType">Type of the snap shot.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TQuery GetQuery(SnapShotType snapShotType, TEntity entity);
    }
}