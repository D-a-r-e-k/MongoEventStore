using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.Domain
{
    [BsonIgnoreExtraElements]
    public class EventSource
    {
        public EventSource()
        {
            Events = new List<Event>();
        }

        [BsonId]
        public ObjectId _id { get; set; }

        public Guid AggregateId { get; set; }
        public int Version { get; set; }

        public List<Event> Events { get; set; }
    }
}
