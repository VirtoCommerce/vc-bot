namespace VirtoCommerce.OrderBot.Bots.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            Customer = new Customer();
        }

        public string UserId { get; set; }

        public Customer Customer { get; set; }
    }
}
