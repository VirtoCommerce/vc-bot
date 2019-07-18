using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.AutoRestClients.CartModuleApi;
using VirtoCommerce.OrderBot.AutoRestClients.CartModuleApi.Models;
using VirtoCommerce.OrderBot.Bots.Models;
using api = VirtoCommerce.OrderBot.AutoRestClients.CartModuleApi.Models;
using dto = VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Builder
{
    public class CartBuilder : ICartBuilder
    {
        private readonly ICartModule _cartModule;
 
        private const string CartName = "Bot cart";

        private Customer _customer;
        private api.ShoppingCart _cart;
        private bool _isDisposed;

        private Customer Customer
        {
            get => _isDisposed ? throw new ObjectDisposedException(GetType().Name) : _customer;
            set => _customer = value ?? throw new ArgumentNullException(nameof(Customer));
        }

        private api.ShoppingCart Cart
        {
            get => _isDisposed ? throw new ObjectDisposedException(GetType().Name) : _cart;
            set => _cart = value;
        }

        public CartBuilder(ICartModule cartModule, Customer customer)
        {
            _cartModule = cartModule;
            Customer = customer;
        }

        public async Task AddCartItemAsync(dto.LineItem lineItem, int quantity)
        {
            await LoadOrCreateNewTransientCart();

            var existingLineItem = Cart.Items.FirstOrDefault(l => l.ProductId == lineItem.ProductId);

            if (existingLineItem != null)
            {
                existingLineItem.Quantity += Math.Max(1, quantity);
            }
            else
            {
                Cart.Items.Add(new api.LineItem
                {
                    CatalogId = lineItem.CatalogId,
                    CategoryId = lineItem.CategoryId,
                    Currency = Customer.Currency,
                    ListPrice = Convert.ToDouble(lineItem.Price),
                    Name = lineItem.Name,
                    ProductId = lineItem.ProductId,
                    Quantity = Math.Max(1, quantity),
                    Sku = lineItem.Code,
                    ImageUrl = lineItem.ImgUrl
                });
            }
        }

        public async Task SaveCartAsync()
        {
            await LoadOrCreateNewTransientCart();

            if (string.IsNullOrEmpty(Cart.Id))
            {
                await _cartModule.CreateAsync(Cart);
            }
            else
            {
                await _cartModule.UpdateAsync(Cart);
            }
        }

        public async Task<dto.ShoppingCart> GetCartAsync()
        {
            await LoadOrCreateNewTransientCart();

            return new dto.ShoppingCart
            {
                Id = Cart.Id,
                Name = Cart.Name,
                Total = Convert.ToDecimal(Cart.Total),
                Currency = Cart.Currency,
                Items = Cart
                    .Items
                    .Select(i => new dto.LineItem
                    {
                        CatalogId = i.CatalogId,
                        CategoryId = i.CategoryId,
                        Code = i.Sku,
                        ImgUrl = i.ImageUrl,
                        Price = Convert.ToDecimal(i.ListPrice),
                        Currency = i.Currency,
                        Name = i.Name,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity ?? 0
                    })
                    .ToArray()
            };
        }

        public async Task RemoveCartAsync()
        {
            await LoadOrCreateNewTransientCart();

            await _cartModule.DeleteCartsAsync(new[] { Cart.Id });
        }


        private async Task<api.ShoppingCart> SearchCartAsync()
        {
            var criteria = new ShoppingCartSearchCriteria
            {
                Currency = Customer.Currency,
                CustomerId = Customer.Id,
                StoreId = Customer.StoreId,
                Name = CartName
            };

            var searchResult = await _cartModule.SearchAsync(criteria);

            return searchResult.Results.FirstOrDefault();
        }

        private api.ShoppingCart CreateCart()
        {
            return new api.ShoppingCart
            {
                CustomerId = Customer.Id,
                Name = CartName,
                StoreId = Customer.StoreId,
                IsAnonymous = false,
                CustomerName = Customer.Name,
                Currency = Customer.Currency,
                Items = new List<api.LineItem>()
            };
        }

        private async Task LoadOrCreateNewTransientCart()
        {
            if (Cart == null)
            {
                Cart = await SearchCartAsync() ?? CreateCart();
            }   
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _customer = null;
            _cart = null;
            _isDisposed = true;
        }
    }
}
