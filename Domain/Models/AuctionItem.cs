using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
    public class AuctionItem
    {
        [BsonId]
        public Guid ItemId { get; set; }

        public Guid OwnerId { get; set; }

        public DateTime AuctionStart { get; set; }

        public DateTime AuctionFinish { get; set; }

        public decimal Value { get; set; }

        public Guid TopBidderId { get; set; }

        public decimal TopBidderValue { get; set; }
    }

    public static class AuctionItemExtensions
    {
        public static bool IsExpired(this AuctionItem item) =>
            DateTime.UtcNow > item.AuctionFinish;
    }
}
