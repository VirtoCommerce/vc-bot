using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Security
{
    public interface IAuthorizationService
    {
        Task<bool> IsAuthorizedAsync(string identifier);

        Task<Customer> GetCustomerAsync(string identifier);
    }
}
