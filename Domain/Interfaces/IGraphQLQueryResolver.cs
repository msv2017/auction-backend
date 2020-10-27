using GraphQL.Types;

namespace Domain.Interfaces
{
    public interface IGraphQLQueryResolver
    {
        void AddFieldsToQuery(IObjectGraphType type);
    }
}
