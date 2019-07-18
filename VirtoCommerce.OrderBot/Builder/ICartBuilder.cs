using System;
using System.Threading.Tasks;
using dto = VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Builder
{
    public interface ICartBuilder : IDisposable
    {
        Task AddCartItemAsync(dto.LineItem lineItem, int quantity);

        Task SaveCartAsync();

        Task RemoveCartAsync();

        Task<dto.ShoppingCart> GetCartAsync();
    }
}
