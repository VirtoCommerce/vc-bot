using Microsoft.Extensions.Options;
using Microsoft.Rest;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.OrderBot.Infrastructure.Autorest
{
    public class VirtoCommerceApiRequestHandler : ServiceClientCredentials
    {
        private readonly PlatformEndpointOptions _options;

        public VirtoCommerceApiRequestHandler(IOptions<PlatformEndpointOptions> options)
        {
            _options = options.Value;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthorization(request);

            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }


        private void AddAuthorization(HttpRequestMessage request)
        {
            if (_options != null)
            {
                var signature = new ApiRequestSignature { AppId = _options.AppId };

                var parameters = new[]
                {
                    new NameValuePair(null, _options.AppId),
                    new NameValuePair(null, signature.TimestampString)
                };

                signature.Hash = HmacUtility.GetHashString(key => new HMACSHA256(key), _options.SecretKey, parameters);

                request.Headers.Authorization = new AuthenticationHeaderValue("HMACSHA256", signature.ToString());
            }
        }
    }
}
