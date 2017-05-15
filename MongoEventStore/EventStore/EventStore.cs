using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoEventStore.Domain;

namespace MongoEventStore.EventStore
{
    public class EventStore
    {
        private readonly IMongoDatabase _database;

        public EventStore()
        {
            IMongoClient client = new MongoClient();
            _database = client.GetDatabase("EventStore");
        }

        private int GetEventSourceVersion(EventSource eventSource)
        {
            if (eventSource == null)
                return 0;

            return eventSource.Version;
        }

        private int GetEventSourceVersion(AlternativeDomain.EventSource eventSource)
        {
            if (eventSource == null)
                return 0;

            return eventSource.Version;
        }

        public EventSource GetAggregateEvents(Guid aggregateId)
        {
            var eventSources = _database.GetCollection<EventSource>("EventSource");

            var eventSource = eventSources.Find(x => x.AggregateId == aggregateId).FirstOrDefault();

            eventSource.Events = eventSource.Events.OrderBy(x => x.Version).ToList();

            return eventSource;
        }

        public void SaveEvents(Guid aggregateId, int expectedVersion, List<Event> events)
        {
            var eventSources = _database.GetCollection<EventSource>("EventSource");

            var currentVersion = eventSources
                .Find(x => x.AggregateId == aggregateId)
                .Project(x => x.Version)
                .FirstOrDefault();

            int initialVersion = currentVersion;

            if (expectedVersion != currentVersion)
                throw new Exception("Concurrency problem");

            foreach (var e in events)
                e.Version = ++currentVersion;

            if (initialVersion != 0)
                eventSources.UpdateOne(x => x.AggregateId == aggregateId,
                    new UpdateDefinitionBuilder<EventSource>()
                        .Inc(x => x.Version, events.Count)
                        .PushEach(x => x.Events, events));
            else
                eventSources.InsertOne(new EventSource
                {
                    AggregateId = aggregateId,
                    Version = currentVersion,
                    Events = events
                });
        }

        public void SaveEventsNotAtomically(Guid aggregateId, int expectedVersion, List<Event> events)
        {
            var eventSources = _database.GetCollection<AlternativeDomain.EventSource>("EventSource2");

            var eventSource = eventSources.FindSync(x => x.AggregateId == aggregateId).FirstOrDefault();

            bool existedAggregate = true;
            if (eventSource == null)
            {
                existedAggregate = false;
                eventSource = new AlternativeDomain.EventSource
                {
                    AggregateId = aggregateId,
                    Version = 0
                };
            }

            var currentVersion = GetEventSourceVersion(eventSource);

            if (expectedVersion != currentVersion)
                throw new Exception("Concurrency problem");

            foreach (var e in events)
                e.Version = ++currentVersion;

            eventSource.Version += events.Count;

            if (existedAggregate)
                eventSources.UpdateOne(x => x.AggregateId == aggregateId,
                    new UpdateDefinitionBuilder<AlternativeDomain.EventSource>()
                        .Set(x => x.Version, eventSource.Version));
            else
                eventSources.InsertOne(eventSource);

            var eventsCollection = _database.GetCollection<AlternativeDomain.Event>("Event");
            eventsCollection.InsertMany(events.Select(x => new AlternativeDomain.Event
            {
                AggregateId = aggregateId,
                Version = x.Version,
                Data = x.Data,
                Date = x.Date
            }));
        }

        public void SaveEventsAtomically(Guid aggregateId, int expectedVersion, List<Event> events)
        {
            var eventsCollection = _database.GetCollection<AlternativeDomain.Event>("Event");

            var currentVersion = eventsCollection
                .Find(x => x.AggregateId == aggregateId)
                .Project(x => x.Version)
                .Sort(new SortDefinitionBuilder<AlternativeDomain.Event>()
                    .Descending(x => x.Version))
                .FirstOrDefault();

            if (expectedVersion != currentVersion)
                throw new Exception("Concurrency problem");

            foreach (var e in events)
                e.Version = ++currentVersion;

            eventsCollection.InsertMany(events.Select(x => new AlternativeDomain.Event
            {
                AggregateId = aggregateId,
                Version = x.Version,
                Data = x.Data,
                Date = x.Date
            }));
        }
    }
}
