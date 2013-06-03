namespace RationalEvs
{
    public interface IVersionableEntity<TId>
    {
        TId Id { get; set; }
        long Version { get; set; }
    }
}