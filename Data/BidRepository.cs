using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using MongoDB.Driver;

namespace Data
{
    public interface IBidRepository
    {
        public Task<List<Bid>> GetAllByItemIdAsync(Guid itemId);

        public Task<List<Bid>> GetAllByUserIdAsync(Guid userId);

        public Task AddAsync(Bid bid);

        public Task RemoveAsync(Bid bid);

        public Task ReplaceAsync(Bid bid);

        public Task RemoveAllByItemIdAsync(Guid itemId);
    }

    public class BidRepository : IBidRepository
    {
        private readonly IMongoCollection<Bid> bids;

        public BidRepository(IBidDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.bids = database.GetCollection<Bid>(settings.BidsCollectionName);
        }

        public async Task<List<Bid>> GetAllByItemIdAsync(Guid itemId) =>
            (await this.bids
                .FindAsync(x => x.ItemId == itemId))
                .ToList();

        public async Task<List<Bid>> GetAllByUserIdAsync(Guid userId) =>
            (await this.bids
                .FindAsync(x => x.UserId == userId))
                .ToList();

        public async Task AddAsync(Bid bid) =>
            await this.bids.InsertOneAsync(bid);

        public async Task RemoveAsync(Bid bid) =>
            await this.bids.DeleteOneAsync(x => x.ItemId == bid.ItemId && x.UserId == bid.UserId);

        public async Task ReplaceAsync(Bid bid) =>
            await this.bids.FindOneAndReplaceAsync(x => x.ItemId == bid.ItemId && x.UserId == bid.UserId, bid);

        public async Task RemoveAllByItemIdAsync(Guid itemId) =>
            await this.bids.DeleteManyAsync(x => x.ItemId == itemId);
    }
}
