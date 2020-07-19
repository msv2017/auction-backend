using System;

namespace Domain
{
    public class AuctionItemDatabaseSettings : IAuctionItemDatabaseSettings
    {
        public string AuctionItemsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IAuctionItemDatabaseSettings
    {
        string AuctionItemsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
