namespace RationalEvs.Services.RefuseEngines.Factories
{
    /// <summary>
    /// Crear los Engines desde el Windsor mediante esta factory, poniendolos Singleton.
    /// </summary>
    public static class RefuseEngineFactory
    {

        /// <summary>
        /// Defaults the refuse engine.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public static IRefuseEngine<TEntity, TId> DefaultRefuseEngine<TEntity, TId>() where TEntity : IVersionableEntity<TId>
        {
            return new NoActionRefuseEngine<TEntity, TId>();
        }


        /// <summary>
        /// Creates the remove event refuse engine.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public static IRefuseEngine<TEntity, TId> CreateRemoveEventRefuseEngine<TEntity, TId>() where TEntity : IVersionableEntity<TId>
        {
            return new RemoveEventRefuseEngine<TEntity, TId>();
        }

        /// <summary>
        /// Creates the remove event refuse engine.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public static IRefuseEngine<TEntity, TId> CreateNoApplyEventRefuseEngine<TEntity, TId>() where TEntity : IVersionableEntity<TId>
        {
            return new NoApplyEventRefuseEngine<TEntity, TId>();
        }
    }
}