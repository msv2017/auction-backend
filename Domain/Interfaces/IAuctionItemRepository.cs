using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAuctionItemRepository
    {
        public Task<List<AuctionItem>> GetAllAsync();

        public Task<List<AuctionItem>> GetAllByOwnerAsync(User owner);

        public Task<AuctionItem> GetByItemIdAsync(Guid id);

        public Task AddAsync(AuctionItem item);

        public Task ReplaceAsync(AuctionItem item);

        Task RemoveAsync(AuctionItem item);

        Task RemoveAllAsync(List<AuctionItem> items);
    }
}
