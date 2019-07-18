using api = VirtoCommerce.OrderBot.AutoRestClients.CatalogModuleApi.Models;
using dto = VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Bots.Models.Converters
{
    public static class CriteriaConverter
    {
        public static api.ProductSearchCriteria ToApiCriteria(this dto.ProductSearchCriteria criteria)
        {
            return new api.ProductSearchCriteria
            {
                Take = criteria.Take,
                SearchPhrase = criteria.SearchPhrase,
                StoreId = criteria.StoreId
            };
        }
    }
}
