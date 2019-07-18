using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.OrderBot.Bots.Middlewares
{
    public class SampleMiddleware : IMiddleware
    {
        public SampleMiddleware()
        {

        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = new CancellationToken())
        {
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
