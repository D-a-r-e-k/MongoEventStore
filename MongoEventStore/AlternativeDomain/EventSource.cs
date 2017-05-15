
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.AlternativeDomain
{
    public class EventSource
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }
}
