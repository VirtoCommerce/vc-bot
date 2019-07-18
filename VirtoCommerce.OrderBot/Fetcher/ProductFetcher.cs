using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.OrderBot.AutoRestClients.PricingModuleApi;
using VirtoCommerce.OrderBot.AutoRestClients.PricingModuleApi.Models;
using VirtoCommerce.OrderBot.Bots.Models;
using VirtoCommerce.OrderBot.Bots.Models.Converters;
using VirtoCommerce.OrderBot.Extensions;
using Product = VirtoCommerce.OrderBot.Bots.Models.Product;

namespace VirtoCommerce.OrderBot.Fetcher
{
    public class ProductFetcher : IProductFetcher
    {
        private readonly ICatalogModuleSearch _catalogModuleSearch;
        private readonly IPricingModule _pricingModule;

        public ProductFetcher(ICatalogModuleSearch catalogModuleSearch, IPricingModule pricingModule)
        {
            _catalogModuleSearch = catalogModuleSearch;
            _pricingModule = pricingModule;
        }

        public async Task<Product[]> GetProductsAsync(ProductSearchCriteria criteria)
        {
            var result = await _catalogModuleSearch.SearchProductsAsync(criteria.ToApiCriteria());

            var products = new List<Product>();

            if (!result.Items.IsNullOrEmpty())
            {
                var evaluationContext = new PriceEvaluationContext
                {
                    ProductIds = result.Items.Select(p => p.Id).ToArray()
                };

                var prices = await _pricingModule.EvaluatePricesAsync(evaluationContext);

                products.AddRange(
                    result
                        .Items
                        .Select(
                            product => product.ToProduct(
                                prices.FirstOrDefault(p => p.ProductId == product.Id)
                                )
                            )
                    );
            }

            return products.ToArray();
        }
    }
}
