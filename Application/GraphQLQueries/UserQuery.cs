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
    public class UserQuery : ObjectGraphType, IGraphQLQueryResolver
    {
        private readonly IUserRepository userRepository;

        public UserQuery(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public void AddFieldsToQuery(IObjectGraphType query)
        {
            this.AddFieldToQuery<UserType, User>(query, "user", nameof(Resolve));
        }

        public async Task<IEnumerable<User>> Resolve(Guid id, string name)
        {
            var users = await this.userRepository.GetAllAysnc();

            return users
                .Where(x => id == Guid.Empty || x.Id == id)
                .Where(x => string.IsNullOrWhiteSpace(name) || x.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
