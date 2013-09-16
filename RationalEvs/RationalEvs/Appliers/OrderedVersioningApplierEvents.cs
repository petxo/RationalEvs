using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RationalEvs.Events;
using RationalEvs.Repositories;

namespace RationalEvs.Appliers
{
    public class OrderedVersioningApplierEvents : ApplierEventsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedVersioningApplierEvents"/> class.
        /// </summary>
        public OrderedVersioningApplierEvents()
            : this(false)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedVersioningApplierEvents"/> class.
        /// </summary>
        /// <param name="notApplyEventPreviousRootVersion">if set to <c>true</c> [not apply event previous root version].</param>
        public OrderedVersioningApplierEvents(bool notApplyEventPreviousRootVersion)
            : base(notApplyEventPreviousRootVersion)
        {
        }

        /// <summary>
        /// Gets the ordered version.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="events">The events.</param>
        /// <param name="version"> </param>
        /// <returns></returns>
        public override IEnumerable<IDomainEvent<TEntity>> GetOrderedEvents<TEntity>(IEnumerable<IDomainEvent<TEntity>> events, long version)
        {
            return events.Where(ev => !NotApplyEventPreviousRootVersion || ev.Version >= version).OrderBy(ev => ev.Version).ToList();
        }

        /// <summary>
        /// Gets the type snap shot.
        /// </summary>
        /// <value>
        /// The type snap shot.
        /// </value>
        public override SnapShotType TypeSnapShot
        {
            get { return SnapShotType.ByVersion; }
        }
    }
}