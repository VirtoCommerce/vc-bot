using VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Builder
{
    public interface ICartBuilderFactory
    {
        ICartBuilder Create(Customer customer);
    }
}
