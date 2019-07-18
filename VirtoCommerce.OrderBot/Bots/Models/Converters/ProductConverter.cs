using System;
using VirtoCommerce.OrderBot.AutoRestClients.PricingModuleApi.Models;
using api = VirtoCommerce.OrderBot.AutoRestClients.CatalogModuleApi.Models;
namespace VirtoCommerce.OrderBot.Bots.Models.Converters
{
    public static class ProductConverter
    {
        public static Product ToProduct(this api.Product product, Price price)
        {
            var actualPrice = Convert.ToDecimal(price?.List) > Convert.ToDecimal(price?.Sale)
                ? Convert.ToDecimal(price?.List)
                : Convert.ToDecimal(price?.Sale);

            return new Product
            {
                Code = product.Code,
                Id = product.Id,
                Currency = price?.Currency,
                Name = product.Name,
                ImageUrl = product.ImgSrc,
                Price = actualPrice,
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId
            };
        }
    }
}
