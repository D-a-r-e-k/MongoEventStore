
using System;
using MongoDB.Bson;

namespace MongoEventStore.AlternativeDomain
{
    public class Event
    {
        public ObjectId _id { get; set; }
        public int Version { get; set; }
        public Guid AggregateId { get; set; }
        public byte[] Data { get; set; }
        public DateTime Date { get; set; }
    }
}
