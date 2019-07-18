namespace VirtoCommerce.OrderBot.Bots.Models
{
    public class LineItem
    {
        public string Code { get; set; }

        public string ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public string CatalogId { get; set; }

        public string CategoryId { get; set; }

        public string ImgUrl { get; set; }

        public int Quantity { get; set; }
    }
}
