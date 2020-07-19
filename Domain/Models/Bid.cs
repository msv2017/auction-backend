using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
    public class Bid
    {
        [BsonId]
        public Guid UserId { get; set; }

        public Guid ItemId { get; set; }

        public decimal Value { get; set; }
    }
}
