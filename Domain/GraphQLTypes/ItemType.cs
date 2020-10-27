using GraphQL.Types;

namespace Domain.GraphQLTypes
{
    public class ItemType : ObjectGraphType<Item>
    {
        public ItemType()
        {
            Name = "item";

            Field(x => x.Id, type: typeof(IdGraphType)).Description("Item Id");
            Field(x => x.DisplayName).Description("Item name");
            Field(x => x.OwnerId, type: typeof(IdGraphType)).Description("Owner Id");
        }
    }
}
