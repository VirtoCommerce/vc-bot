using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Models;
using VirtoCommerce.OrderBot.Builder;
using VirtoCommerce.OrderBot.Fetcher;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public class AddToCartDialog : ComponentDialog
    {
        private readonly ConversationState _conversationState;
        private readonly ICartBuilderFactory _cartBuilderFactory;
        private readonly IProductFetcher _productFetcher;
        private readonly ViewCartDialog _viewCartDialog;

        private const string Back = "Back to search";
        private const string ViewCart = "View cart";

        public AddToCartDialog(
            ICartBuilderFactory cartBuilderFactory,
            IProductFetcher productFetcher,
            ConversationState conversationState,
            ViewCartDialog viewCartDialog) 
            : base(nameof(AddToCartDialog))
        {
            _conversationState = conversationState;
            _cartBuilderFactory = cartBuilderFactory;
            _viewCartDialog = viewCartDialog;
            _productFetcher = productFetcher;

            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(_viewCartDialog);
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RequestQuantityAsync,
                ConfirmAsync,
                ViewCartPromptAsync,
                BackOrViewCartAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestQuantityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var productCode = (string) stepContext.Options;
            stepContext.Values["code"] = productCode;

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter quantity"),
                RetryPrompt = MessageFactory.Text("Please enter a correct integer number")
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var code = (string) stepContext.Values["code"];
            var quantity = (int) stepContext.Result;

            var productSearchCriteria = new ProductSearchCriteria
            {
                SearchPhrase = code
            };

            var products = await _productFetcher.GetProductsAsync(productSearchCriteria);

            if (products.Length != 0)
            {
                var product = products.First();
                var userProfileAccessor = _conversationState.CreateProperty<UserProfile>(nameof(UserProfile));
                var userProfile = await userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                using (var cartBuilder = _cartBuilderFactory.Create(userProfile.Customer))
                {
                    var lineItem = new LineItem
                    {
                        CatalogId = product.CatalogId,
                        CategoryId = product.CategoryId,
                        Code = product.Code,
                        Currency = product.Currency,
                        ImgUrl = product.ImageUrl,
                        Name = product.Name,
                        Price = product.Price,
                        ProductId = product.Id
                    };

                    await cartBuilder.AddCartItemAsync(lineItem, quantity);
                    await cartBuilder.SaveCartAsync();
                }
            }

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ViewCartPromptAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text("What you like to do?"),
                Choices = new []
                {
                    new Choice(ViewCart), 
                    new Choice(Back)
                }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> BackOrViewCartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = ((FoundChoice) stepContext.Result)?.Value;

            if (string.Equals(result, ViewCart, StringComparison.InvariantCultureIgnoreCase))
            {
                return await stepContext.BeginDialogAsync(_viewCartDialog.GetType().Name, cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
