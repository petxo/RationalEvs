namespace RationalEvs.Fsm.Configuration.Loaders
{
    public interface IFsmLoader
    {
        GraphFsm LoadFrom(string filename);
    }
}