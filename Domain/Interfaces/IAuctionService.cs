using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAuctionService
    {
        Task<List<AuctionItem>> GetItemsAsync();

        Task PutItemOnActionAsync(Guid itemId, decimal value, DateTime expiryDate, Guid userId);

        Task RemoveItemFromAuctionAsync(Guid itemId, Guid userId);

        Task BidOnItemAsync(Guid itemId, decimal value, Guid userId);

        Task RemoveBidOnItemAsync(Guid itemId, Guid userId);
    }
}
