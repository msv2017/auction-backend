using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Domain;

namespace Application
{
    public interface IAdminService
    {
        Task InitializeAsync();
    }

    public class AdminService: IAdminService
    {
        private readonly IUserRepository userRepository;
        private readonly IItemRepository itemRepository;

        public AdminService(
            IUserRepository userRepository,
            IItemRepository itemRepository)
        {
            this.userRepository = userRepository;
            this.itemRepository = itemRepository;
        }

        public async Task InitializeAsync()
        {
            var admin = new User("Admin", "admin", "password");

            var users = new[]
            {
                admin,
                new User("John Constantine", "john.constantine", "john777"),
                new User("Martian Manhunter", "martian.man", "barsum222"),
                new User("Wonder Woman", "wonder.woman", "shield333"),
                new User("Green Lantern", "green.lantern", "oamyplanet111"),
                new User("Lex Luthor", "lex.luthor", "lextherobot555")
            };

            var items = new[]
            {
                new Item("Thor's Hammer", admin),
                new Item("Iron man's gauntlet", admin),
                new Item("Infinity stones", admin)
            };

            foreach(var user in users)
            {
                await this.userRepository.AddUserAsync(user);
            }

            foreach(var item in items)
            {
                await this.itemRepository.AddAsync(item);
            }
        }
    }
}
