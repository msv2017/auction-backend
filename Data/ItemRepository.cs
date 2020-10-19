using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using MongoDB.Driver;

namespace Data
{
    public class ItemRepository : IItemRepository
    {
        private readonly IMongoCollection<Item> items;

        public ItemRepository(IItemDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.items = database.GetCollection<Item>(settings.ItemsCollectionName);
        }

        public async Task<List<Item>> GetAllAsync() =>
            (await this.items
                .FindAsync(x => true))
                .ToList();

        public async Task<List<Item>> GetAllByOwnerAsync(User owner) =>
            (await this.items
                .FindAsync(x => x.OwnerId == owner.Id))
                .ToList();

        public async Task<Item> GetByIdAsync(Guid id) =>
            (await this.items
                .FindAsync(x => x.Id == id))
                .FirstOrDefault();

        public async Task AddAsync(Item item) =>
            await this.items
                .InsertOneAsync(item);

        public async Task ReplaceAsync(Item item) =>
            await this.items
                .FindOneAndReplaceAsync(x => x.Id == item.Id, item);
    }
}
