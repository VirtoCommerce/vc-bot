using Microsoft.Bot.Builder;

namespace VirtoCommerce.OrderBot.Bots.Middlewares.Injector
{
    public interface IMiddlewareInjector
    {
        IMiddlewareInjector AddMiddleware(IMiddleware middleware);
        void Inject(BotAdapter botFrameworkAdapter);
    }
}
