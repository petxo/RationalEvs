using System.Reflection;
using MongoDB.Bson.Serialization;
using RationalEvs.Entities;
using MongoDB.Bson;

namespace RationalEvs.Sql
{
    public class Translator
    {

        public static Translator Instance { get; private set; }

        static Translator()
        {
            Instance = new Translator();
        }

        public void TranslateToDomain<TEntity>(EntityEventSource<TEntity, long> domain, EntityEventSourceWrapper entity) 
            where TEntity : IVersionableEntity<long>, new()
        {
            domain.Id = entity.Id;
            domain.State = entity.State;
            domain.Status = entity.Status;
            domain.ProcessingAt = entity.ProcessingAt;
            domain.ProcessingBy = entity.ProcessingBy;
            if (entity.SnapShot != null)
            {
                domain.SnapShot = (TEntity) BsonSerializer.Deserialize(entity.SnapShot, typeof (TEntity));
            }

            foreach (var eventWrapper in entity.Events)
            {
                //MongoDB.Bson.Serialization.BsonSerializer.
            }
        }

    }
}