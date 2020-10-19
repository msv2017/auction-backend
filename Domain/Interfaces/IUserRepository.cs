using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAysnc();

        Task<User> GetUserAsync(string username, string password);

        Task<User> GetUserByIdAsync(Guid id);

        Task AddUserAsync(User user);

    }
}
