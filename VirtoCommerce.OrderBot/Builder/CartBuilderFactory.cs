using System;
using VirtoCommerce.OrderBot.AutoRestClients.CartModuleApi;
using VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Builder
{
    public class CartBuilderFactory : ICartBuilderFactory
    {
        private readonly ICartModule _cartModule;

        public CartBuilderFactory(ICartModule cartModule)
        {
            _cartModule = cartModule;
        }

        public ICartBuilder Create(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            return new CartBuilder(_cartModule, customer);
        }
    }
}
