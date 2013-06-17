using System.Collections.Generic;
using MongoDB.Driver;
using RationalEvs.Events;
using RationalEvs.Repositories;

namespace RationalEvs.Appliers
{
    public abstract class ApplierEventsBase : IApplierEvents
    {

        private readonly bool _notApplyEventPreviousRootVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplierEventsBase"/> class.
        /// </summary>
        /// <param name="notApplyEventPreviousRootVersion">if set to <c>true</c> [not apply event previous root version].</param>
        protected ApplierEventsBase(bool notApplyEventPreviousRootVersion)
        {
            _notApplyEventPreviousRootVersion = notApplyEventPreviousRootVersion;
        }

        public abstract SnapShotType TypeSnapShot { get; }

        /// <summary>
        /// Gets a value indicating whether [not apply event previous root version].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [not apply event previous root version]; otherwise, <c>false</c>.
        /// </value>
        protected bool NotApplyEventPreviousRootVersion
        {
            get { return _notApplyEventPreviousRootVersion; }
        }

        /// <summary>
        /// Gets the ordered version.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="events">The events.</param>
        /// <param name="version"> </param>
        /// <returns></returns>
        public abstract IEnumerable<IDomainEvent<TEntity>> GetOrderedEvents<TEntity>(IEnumerable<IDomainEvent<TEntity>> events, long version);

    }
}