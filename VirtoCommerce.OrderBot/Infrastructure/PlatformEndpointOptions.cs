using System;

namespace VirtoCommerce.OrderBot.Infrastructure
{
    public class PlatformEndpointOptions
    {
        public Uri Url { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan RequestTimeout { get; set; }
    }
}
