using System;

namespace Api.Models
{
    public class AuctionItemDto
    {
        public decimal Value { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
