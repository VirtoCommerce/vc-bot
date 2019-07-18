namespace VirtoCommerce.OrderBot.Bots.Models
{
    public class ProductSearchCriteria
    {
        public string SearchPhrase { get; set; }

        public string StoreId { get; set; }

        public int Take { get; set; } = 20;
    }
}
