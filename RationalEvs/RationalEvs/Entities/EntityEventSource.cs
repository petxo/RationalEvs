using System;
using System.Collections.Generic;
using BteamMongoDB;
using MongoDB.Bson.Serialization.Attributes;
using RationalEvs.Events;

namespace RationalEvs.Entities
{
    public class EntityEventSource<TEntity, TId> : IMongoEntity<TId> where TEntity : IVersionableEntity<TId>, new()
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEventSource&lt;TEntity&gt;"/> class.
        /// </summary>
        public EntityEventSource()
        {
            Version = 0;
            Events = new List<IDomainEvent<TEntity>>();
            History = new List<HistoricEntity<TEntity>>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [BsonId]
        public TId Id { get; set; }

        /// <summary>
        /// Gets or sets the history.
        /// </summary>
        /// <value>
        /// The history.
        /// </value>
        public IList<HistoricEntity<TEntity>> History { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public TEntity SnapShot { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the processing at.
        /// </summary>
        /// <value>
        /// The processing at.
        /// </value>
        public DateTime ProcessingAt { get; set; }

        /// <summary>
        /// Gets or sets the processing by.
        /// </summary>
        /// <value>
        /// The processing by.
        /// </value>
        public string ProcessingBy { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>The events.</value>
        public IList<IDomainEvent<TEntity>> Events { get; set; }

        /// <summary>
        /// Gets or sets the applied events.
        /// </summary>
        /// <value>
        /// The applied events.
        /// </value>
        public IList<IDomainEvent<TEntity>> AppliedEvents { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class HistoricEntity<TEntity>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricEntity&lt;TEntity&gt;"/> class.
        /// </summary>
        public HistoricEntity()
        {
            AppliedEvent = new List<IDomainEvent<TEntity>>();
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the applied event.
        /// </summary>
        /// <value>
        /// The applied event.
        /// </value>
        public IList<IDomainEvent<TEntity>> AppliedEvent { get; set; }

        /// <summary>
        /// Gets or sets the last event applied.
        /// </summary>
        /// <value>
        /// The last event applied.
        /// </value>
        public IDomainEvent<TEntity> LastEventApplied { get; set; }

        /// <summary>
        /// Gets or sets the processed at.
        /// </summary>
        /// <value>
        /// The processed at.
        /// </value>
        public DateTime ProcessedAt { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public long Version { get; set; }
    }
}