using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Application
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(string username, string password);

        Task<List<Item>> GetUserItemsAsync(Guid userId);

        Task<List<AuctionItem>> GetUserAuctionItemsAsync(Guid userId);

        Task<List<Bid>> GetUserBidsAsync(Guid userId);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings appSettings;
        private readonly IUserRepository userRepository;
        private readonly IItemRepository itemRepository;
        private readonly IAuctionItemRepository auctionItemRepository;
        private readonly IBidRepository bidRepository;

        public UserService(
            IOptions<AppSettings> appSettings,
            IUserRepository userRepository,
            IItemRepository itemRepository,
            IAuctionItemRepository auctionItemRepository,
            IBidRepository bidRepository)
        {
            this.appSettings = appSettings.Value;
            this.userRepository = userRepository;
            this.itemRepository = itemRepository;
            this.auctionItemRepository = auctionItemRepository;
            this.bidRepository = bidRepository;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await this.userRepository.GetUserAsync(username, password);

            if (user == null)
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        public async Task<List<Item>> GetUserItemsAsync(Guid userId)
        {
            var user = await this.userRepository.GetUserByIdAsync(userId);
            var items = await this.itemRepository.GetAllByOwnerAsync(user);

            return items;
        }

        public async Task<List<AuctionItem>> GetUserAuctionItemsAsync(Guid userId)
        {
            var user = await this.userRepository.GetUserByIdAsync(userId);
            var auctionItems = await this.auctionItemRepository.GetAllByOwnerAsync(user);

            return auctionItems;
        }

        public async Task<List<Bid>> GetUserBidsAsync(Guid userId)
        {
            var userItems = await this.GetUserItemsAsync(userId);
            var userBids = await this.bidRepository.GetAllByUserIdAsync(userId);

            var ownedItemIds = userItems.Select(x => x.Id).ToHashSet();

            return userBids.Where(x => !ownedItemIds.Contains(x.ItemId)).ToList();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(this.appSettings.ExpirationPeriodInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
