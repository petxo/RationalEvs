namespace RationalEvs.Services.RefuseEngines
{
    public delegate void RefusedSuccessEventHandler<TEntity, TId>(object sender, RefusedSuccessEventHandlerArgs<TEntity, TId> args) where TEntity : IVersionableEntity<TId>;
}