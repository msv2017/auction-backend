using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
    public class Item
    {
        [BsonId]
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public Guid OwnerId { get; set; }

        public Item(string name, User owner)
        {
            this.Id = Guid.NewGuid();
            this.DisplayName = name;
            this.OwnerId = owner.Id;
        }
    }
}
