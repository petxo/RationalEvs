﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bteam.NHibernate;

namespace RationalEvs.Sql
{

    public class EntityEventSourceWrapper : IIdentificableEntity<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEventSourceWrapper"/> class.
        /// </summary>
        public EntityEventSourceWrapper()
        {
            ProcessingAt = DateTime.UtcNow;
            Events = new List<EventWrapper>();
        }

        public virtual long Id { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public virtual byte[] SnapShot { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public virtual long Version { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public virtual string State { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public virtual string Status { get; set; }

        /// <summary>
        /// Gets or sets the processing at.
        /// </summary>
        /// <value>
        /// The processing at.
        /// </value>
        public virtual DateTime ProcessingAt { get; set; }

        /// <summary>
        /// Gets or sets the processing by.
        /// </summary>
        /// <value>
        /// The processing by.
        /// </value>
        public virtual string ProcessingBy { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public virtual IList<EventWrapper> Events { get; set; }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        public virtual void AddEvent(EventWrapper eventWrapper)
        {
            Events.Add(eventWrapper);
            eventWrapper.Entity = this;
        }

        /// <summary>
        /// Clears the events.
        /// </summary>
        public virtual void ClearEvents()
        {
            foreach (var @event in Events)
            {
                @event.Entity = null;
            }
            Events.Clear();
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="version">The version.</param>
        public virtual void RemoveEvent(string type, long version)
        {
            var firstOrDefault = Events.FirstOrDefault(wrapper => wrapper.Type == type && wrapper.Version == version);
            if (firstOrDefault != null)
            {
                Events.Remove(firstOrDefault);
                firstOrDefault.Entity = null;
            }
        }
    }
}