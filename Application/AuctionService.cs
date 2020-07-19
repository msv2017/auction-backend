using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Domain;

namespace Application
{
    public interface IAuctionService
    {
        Task<List<AuctionItem>> GetItemsAsync();

        Task PutItemOnActionAsync(Guid itemId, decimal value, DateTime expiryDate, Guid userId);

        Task RemoveItemFromAuctionAsync(Guid itemId, Guid userId);

        Task BidOnItemAsync(Guid itemId, decimal value, Guid userId);

        Task RemoveBidOnItemAsync(Guid itemId, Guid userId);
    }

    public class AuctionService : IAuctionService
    {
        private readonly IItemRepository itemRepository;
        private readonly IAuctionItemRepository auctionItemRepository;
        private readonly IBidRepository bidRepository;

        public AuctionService(
            IItemRepository itemRepository,
            IAuctionItemRepository auctionItemRepository,
            IBidRepository bidRepository)
        {
            this.itemRepository = itemRepository;
            this.auctionItemRepository = auctionItemRepository;
            this.bidRepository = bidRepository;
        }

        public async Task<List<AuctionItem>> GetItemsAsync()
        {
            var items = await this.auctionItemRepository.GetAllAsync();

            items = await this.ValidateAuctionItemsAsync(items);

            return items;
        }

        public async Task PutItemOnActionAsync(Guid itemId, decimal value, DateTime expiryDate, Guid userId)
        {
            var item = await this.itemRepository.GetByIdAsync(itemId)
                ?? throw new Exception("Item not found");

            if (item.OwnerId != userId)
            {
                throw new Exception("Only owner can put item on the auction");
            }

            var auctionItem = await this.auctionItemRepository.GetByItemIdAsync(item.Id);

            if (auctionItem != null)
            {
                throw new Exception("The item has already been auctioned");
            }

            auctionItem = new AuctionItem
            {
                ItemId = item.Id,
                OwnerId = userId,
                AuctionStart = DateTime.UtcNow,
                AuctionFinish = expiryDate,
                Value = value,
                TopBidderId = Guid.Empty,
                TopBidderValue = value
            };

            if (auctionItem.IsExpired())
            {
                throw new Exception("Expiry date is in the past");
            }

            await this.auctionItemRepository.AddAsync(auctionItem);
        }

        public async Task RemoveItemFromAuctionAsync(Guid itemId, Guid userId)
        {
            var auctionItem = await this.auctionItemRepository.GetByItemIdAsync(itemId)
                ?? throw new Exception("Auction item not found");

            if (auctionItem.OwnerId != userId)
            {
                throw new Exception("Only owner can remove item from the auction");
            }

            await this.auctionItemRepository.RemoveAsync(auctionItem);
            await this.bidRepository.RemoveAllByItemIdAsync(itemId);
        }

        public async Task BidOnItemAsync(Guid itemId, decimal value, Guid userId)
        {
            var auctionItem = await this.auctionItemRepository.GetByItemIdAsync(itemId)
                ?? throw new Exception("Auction item not found");

            if (auctionItem.OwnerId == userId)
            {
                throw new Exception("Owner is not allowed to put bid on owned item");
            }

            if (auctionItem.IsExpired())
            {
                throw new Exception("Auction item is expired");
            }

            if (value < auctionItem.TopBidderValue)
            {
                throw new Exception("Only higher bid value is allowed");
            }

            var bid = (await this.bidRepository.GetAllByItemIdAsync(itemId))
                .FirstOrDefault(x => x.UserId == userId);

            if (bid == null)
            {
                bid = new Bid { ItemId = itemId, UserId = userId, Value = value };
                await this.bidRepository.AddAsync(bid);
            }
            else
            {
                bid.Value = value;
                await this.bidRepository.ReplaceAsync(bid);
            }

            await this.InvalidateBidsAsync(auctionItem);
        }

        public async Task RemoveBidOnItemAsync(Guid itemId, Guid userId)
        {
            var auctionItem = await this.auctionItemRepository.GetByItemIdAsync(itemId)
                ?? throw new Exception("Auction item not found");

            if (auctionItem.IsExpired())
            {
                throw new Exception("Auction item is expired");
            }

            var bid = (await this.bidRepository.GetAllByItemIdAsync(itemId))
                .FirstOrDefault(x => x.UserId == userId)
                ?? throw new Exception("Bid is not found");

            await this.bidRepository.RemoveAsync(bid);

            await this.InvalidateBidsAsync(auctionItem);
        }

        private async Task InvalidateBidsAsync(AuctionItem auctionItem)
        {
            var bids = await this.bidRepository.GetAllByItemIdAsync(auctionItem.ItemId);
            var topBidder = bids.OrderBy(x => x.Value).LastOrDefault();

            auctionItem.TopBidderId = topBidder?.UserId ?? Guid.Empty;
            auctionItem.TopBidderValue = topBidder?.Value ?? auctionItem.Value;

            await this.auctionItemRepository.ReplaceAsync(auctionItem);
        }

        private async Task<List<AuctionItem>> ValidateAuctionItemsAsync(List<AuctionItem> items)
        {
            var auctionFinishedItems = items.Where(x => x.IsExpired()).ToList();

            if (auctionFinishedItems.Any())
            {
                await this.auctionItemRepository.RemoveAllAsync(auctionFinishedItems);
            }

            foreach (var auctionItem in auctionFinishedItems)
            {
                await this.bidRepository.RemoveAllByItemIdAsync(auctionItem.ItemId);

                if (auctionItem.TopBidderId == Guid.Empty)
                {
                    continue;
                }

                var item = await this.itemRepository.GetByIdAsync(auctionItem.ItemId);

                item.OwnerId = auctionItem.TopBidderId;

                await this.itemRepository.ReplaceAsync(item);
            }

            var auctionFinishedItemIds = auctionFinishedItems.Select(x => x.ItemId).ToHashSet();

            return items.Where(x => !auctionFinishedItemIds.Contains(x.ItemId)).ToList();
        }
    }
}
