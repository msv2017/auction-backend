using System.Collections.Generic;
using Domain.Interfaces;
using GraphQL.Types;

namespace Application.GraphQLQueries
{
    public class GraphQLSchema : Schema, ISchema
    {
        public GraphQLSchema(IEnumerable<IGraphQLQueryResolver> resolvers)
        {
            var query = new ObjectGraphType();

            foreach (var resolver in resolvers)
            {
                resolver.AddFieldsToQuery(query);
            }

            this.Query = query;
        }
    }
}
