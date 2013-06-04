namespace RationalEvs.Services
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void ActionStateNotificationHandler<TEntity>(
        object sender, ActionStateNotificationArgs<TEntity> args);
}