using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IItemRepository
    {
        public Task<List<Item>> GetAllAsync();

        public Task<List<Item>> GetAllByOwnerAsync(User owner);

        public Task<Item> GetByIdAsync(Guid id);

        public Task AddAsync(Item item);

        public Task ReplaceAsync(Item item);
    }
}
