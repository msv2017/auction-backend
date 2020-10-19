using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using MongoDB.Driver;

namespace Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> users;

        public UserRepository(IUserDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<List<User>> GetAllAysnc() =>
            (await this.users
                .FindAsync(x => true))
                .ToList();

        public async Task<User> GetUserAsync(string username, string password) =>
             (await this.users
                .FindAsync(x => x.Username == username && x.Password == password))
                .FirstOrDefault();

        public async Task<User> GetUserByIdAsync(Guid id) =>
            (await this.users
                .FindAsync(x => x.Id == id))
                .FirstOrDefault();

        public async Task AddUserAsync(User user) =>
            await this.users
                .InsertOneAsync(user);

    }
}
