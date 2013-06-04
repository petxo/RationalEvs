namespace RationalEvs.Services
{
    public delegate void EnityNotExistNotificationHandler<TId>(object sender, EnityNotExistNotificationHandlerArgs<TId> args);
}