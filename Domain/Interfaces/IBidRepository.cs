using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
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
}
