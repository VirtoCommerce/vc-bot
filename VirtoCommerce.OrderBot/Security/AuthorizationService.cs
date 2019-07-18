using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.OrderBot.AutoRestClients.CustomerModuleApi.Models;
using VirtoCommerce.OrderBot.AutoRestClients.StoreModuleApi;
using VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Security
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ICustomerModule _customerModuleApi;
        private readonly IStoreModule _storeModule;

        private readonly Customer _customer = new Customer();

        public AuthorizationService(ICustomerModule customerModule, IStoreModule storeModule)
        {
            _customerModuleApi = customerModule;
            _storeModule = storeModule;
        }

        public async Task<bool> IsAuthorizedAsync(string identifier)
        {
            var criteria = new MembersSearchCriteria
            {
                SearchPhrase = $"botusername:{identifier}"
            };

            var result = await _customerModuleApi.SearchContactsAsync(criteria);
            var vcCustomer = result.Results.FirstOrDefault();

            if (vcCustomer != null)
            {
                
                _customer.Id = vcCustomer.Id;
                _customer.StoreId = vcCustomer.SecurityAccounts.FirstOrDefault()?.StoreId;
                _customer.Name = vcCustomer.Name;

                if (!string.IsNullOrEmpty(_customer.StoreId))
                {
                    var store = await _storeModule.GetStoreByIdAsync(_customer.StoreId);
                    _customer.Currency = store.DefaultCurrency;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Customer> GetCustomerAsync(string identifier)
        {
            if (string.IsNullOrEmpty(_customer.Id))
            {
                await IsAuthorizedAsync(identifier);
            }

            return _customer;
        }
    }
}
