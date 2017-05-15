using System;
using System.Collections.Generic;
using MongoEventStore.Domain;
using MongoEventStore.Utils;

namespace MongoEventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventStore = new EventStore.EventStore();

            var aggregateId = new Guid("0eaf40de-6481-4895-a654-e6bea1c6a594");

            // x10
            //var eventsToBeInserted = new List<Event>
            //{
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event1"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event2"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event3"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event4"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event5"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event6"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event7"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event8"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event9"),
            //        Date = DateTime.Now
            //    },
            //    new Event
            //    {
            //        Data = ProtoSerializer.Serialize("event10"),
            //        Date = DateTime.Now
            //    }
            //};

            //// x2
            var eventsToBeInserted = new List<Event>
            {
                new Event
                {
                    Data = ProtoSerializer.Serialize("event1"),
                    Date = DateTime.Now
                },
                new Event
                {
                    Data = ProtoSerializer.Serialize("event2"),
                    Date = DateTime.Now
                }
            };

            for (int i = 0; i < 15810; ++i)
            {
                int expectedVersion = 2 * i;

                eventStore.SaveEventsNotAtomically(aggregateId, expectedVersion,
                    eventsToBeInserted);

            }          

            Console.WriteLine("Work is done.");
        }
    }
}
