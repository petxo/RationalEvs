using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace RationalEvs.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public class MongoQuerySnapshotBuilder<TEntity, TId> : IQuerySnapshotBuilder<IMongoQuery, TEntity>
        where TEntity : IVersionableEntity<TId>
    {
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <param name="snapShotType">Type of the snap shot.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public IMongoQuery GetQuery(SnapShotType snapShotType, TEntity entity)
        {
            return snapShotType == SnapShotType.ById
                       ? Query.EQ("_id", BsonDocumentWrapper.Create(entity.Id))
                       : Query.And(Query.EQ("_id", BsonDocumentWrapper.Create(entity.Id)),
                                   Query.Or(Query.NotExists("Version"),
                                            Query.LTE("Version", BsonDocumentWrapper.Create(entity.Version)))
                             );
        }
    }
}