using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.Domain
{
    [BsonIgnoreExtraElements]
    public class Event
    {
        public int Version { get; set; }
        public byte[] Data { get; set; }
        public DateTime Date { get; set; }
    }
}
