using GraphQL.Types;

namespace Domain.GraphQLTypes
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "user";

            Field(x => x.Id, type: typeof(IdGraphType)).Description("Item Id");
            Field(x => x.DisplayName).Description("User display name");
            Field(x => x.Username).Description("User name");
        }
    }
}
