using Microsoft.Bot.Builder;
using System.Collections.Generic;

namespace VirtoCommerce.OrderBot.Bots.Middlewares.Injector
{
    public class MiddlewareInjector : IMiddlewareInjector
    {
        protected ICollection<IMiddleware> Middlewares = new List<IMiddleware>();

        public virtual IMiddlewareInjector AddMiddleware(IMiddleware middleware)
        {
            Middlewares.Add(middleware);
            return this;
        }

        public virtual void Inject(BotAdapter adapter)
        {
            foreach (var middleware in Middlewares)
            {
                adapter.Use(middleware);
            }
        }
    }
}
