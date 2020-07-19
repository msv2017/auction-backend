using System;

namespace Domain
{
    public class BidDatabaseSettings : IBidDatabaseSettings
    {
        public string BidsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBidDatabaseSettings
    {
        string BidsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
