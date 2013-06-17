using NHibernate.Criterion;
using RationalEvs.Repositories;

namespace RationalEvs.Sql
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public class SqlQuerySnapshotBuilder<TEntity, TId> : IQuerySnapshotBuilder<ICriterion, TEntity>
        where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <param name="snapShotType">Type of the snap shot.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public ICriterion GetQuery(SnapShotType snapShotType, TEntity entity)
        {
            return snapShotType == SnapShotType.ById
                       ? Restrictions.Eq("Id", entity.Id)
                       : Restrictions.And(Restrictions.Eq("Id", entity.Id), Restrictions.Le("Version", entity.Version));
        }
    }
}