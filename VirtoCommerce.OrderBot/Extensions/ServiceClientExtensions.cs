using Microsoft.Rest;
using System;

namespace VirtoCommerce.OrderBot.Extensions
{
    public static class ServiceClientExtensions
    {
        public static T DisableRetries<T>(this T client) where T : ServiceClient<T>
        {
            client.SetRetryPolicy(null);
            return client;
        }

        public static T WithTimeout<T>(this T client, TimeSpan timeout)
            where T : ServiceClient<T>
        {
            client.HttpClient.Timeout = timeout;
            return client;
        }
    }
}
