using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Fetcher
{
    public interface IProductFetcher
    {
        Task<Product[]> GetProductsAsync(ProductSearchCriteria criteria);
    }
}
