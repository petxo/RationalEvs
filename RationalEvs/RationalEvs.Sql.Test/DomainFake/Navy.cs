namespace RationalEvs.Sql.Test.DomainFake
{
    public class Navy : IVersionableEntity<long>
    {
        public long Id { get; set; }
        public long Version { get; set; }
    }
}