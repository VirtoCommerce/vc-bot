using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector;
using VirtoCommerce.OrderBot.Bots.Models.Converters;
using VirtoCommerce.OrderBot.Fetcher;
using dto = VirtoCommerce.OrderBot.Bots.Models;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public class CatalogDialog : InterceptorExtendedBaseDialog
    {
        private readonly IProductFetcher _productFetcher;
        private readonly ConversationState _conversationState;

        public CatalogDialog(
            IMessageInterceptor messageInterceptor,
            IProductFetcher productFetcher,
            SearchDialog searchDialog,
            ConversationState conversationState
            ) 
            : base(nameof(CatalogDialog), messageInterceptor)
        {
            _productFetcher = productFetcher;
            _conversationState = conversationState;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GreetingsMessageAsync
            }));
            AddDialog(searchDialog);

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GreetingsMessageAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfileAccessor = _conversationState.CreateProperty<dto.UserProfile>(nameof(dto.UserProfile));
            var userProfile = await userProfileAccessor.GetAsync(stepContext.Context, () => new dto.UserProfile(), cancellationToken);

            var products = await _productFetcher.GetProductsAsync(new dto.ProductSearchCriteria{ StoreId = userProfile.Customer.StoreId });

            var cards = products.GetCards();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Recommended products"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(cards.Select(c => c.ToAttachment())), cancellationToken);
            
            return await stepContext.BeginDialogAsync(nameof(SearchDialog), cancellationToken: cancellationToken);
        }
    }
}
