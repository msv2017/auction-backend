using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Extensions;
using Domain.GraphQLTypes;
using Domain.Interfaces;
using GraphQL.Types;

namespace Application.GraphQLQueries
{
    public class ItemQuery : ObjectGraphType, IGraphQLQueryResolver
    {
        private readonly IItemRepository itemRepository;

        public ItemQuery(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public void AddFieldsToQuery(IObjectGraphType query)
        {
            this.AddFieldToQuery<ItemType, Item>(query, "item", nameof(Resolve));
        }

        public async Task<IEnumerable<Item>> Resolve(Guid id, string name)
        {
            var items = await this.itemRepository.GetAllAsync();

            return items
                .Where(x => id == Guid.Empty || x.Id == id)
                .Where(x => string.IsNullOrWhiteSpace(name) || x.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
