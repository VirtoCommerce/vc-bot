namespace VirtoCommerce.OrderBot.Bots.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Code { get; set; }

        public string Currency { get; set; }
        
        public string CatalogId { get; set; }

        public string CategoryId { get; set; }
    }
}
