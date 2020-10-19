using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using MongoDB.Driver;

namespace Data
{
    public class AuctionItemRepository : IAuctionItemRepository
    {
        private readonly IMongoCollection<AuctionItem> items;

        public AuctionItemRepository(IAuctionItemDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.items = database.GetCollection<AuctionItem>(settings.AuctionItemsCollectionName);
        }

        public async Task<List<AuctionItem>> GetAllAsync() =>
            (await this.items
                .FindAsync(x => true))
                .ToList();

        public async Task<List<AuctionItem>> GetAllByOwnerAsync(User owner) =>
            (await this.items
                .FindAsync(x => x.OwnerId == owner.Id))
                .ToList();

        public async Task<AuctionItem> GetByItemIdAsync(Guid itemId) =>
            (await this.items
                .FindAsync(x => x.ItemId == itemId))
                .FirstOrDefault();

        public async Task AddAsync(AuctionItem item) =>
            await this.items
                .InsertOneAsync(item);

        public async Task ReplaceAsync(AuctionItem item) =>
            await this.items
                .FindOneAndReplaceAsync(x => x.ItemId == item.ItemId, item);

        public async Task RemoveAsync(AuctionItem item) =>
            await this.items
                .DeleteOneAsync(x => x.ItemId == item.ItemId);

        public async Task RemoveAllAsync(List<AuctionItem> items)
        {
            var ids = items.Select(x => x.ItemId).ToHashSet();

            await this.items.DeleteManyAsync(x => ids.Contains(x.ItemId));
        }
    }
}
