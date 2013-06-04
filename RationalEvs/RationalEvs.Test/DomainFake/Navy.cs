using MongoDB.Bson;

namespace RationalEvs.Test.DomainFake
{
    public class Navy: IVersionableEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public long Version { get; set; }
    }
}