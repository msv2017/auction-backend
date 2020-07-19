using System;
namespace Domain.Settings
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public int ExpirationPeriodInMinutes { get; set; }
    }
}
