using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(string username, string password);

        Task<List<Item>> GetUserItemsAsync(Guid userId);

        Task<List<AuctionItem>> GetUserAuctionItemsAsync(Guid userId);

        Task<List<Bid>> GetUserBidsAsync(Guid userId);
    }
}
