using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector;
using VirtoCommerce.OrderBot.Bots.Models;
using VirtoCommerce.OrderBot.Bots.Models.Converters;
using VirtoCommerce.OrderBot.Builder;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public class ViewCartDialog : InterceptorExtendedBaseDialog
    {
        private const string CreateOrder = "Yes, please";
        private const string BackToSearch = "No, back to search";

        private readonly ICartBuilderFactory _cartBuilderFactory;
        private readonly ConversationState _conversationState;
        private readonly IOrderModule _orderModule;

        public ViewCartDialog(
            IMessageInterceptor messageInterceptor,
            ICartBuilderFactory cartBuilderFactory, 
            IOrderModule orderModule,
            ConversationState conversationState
            )
            : base(nameof(ViewCartDialog), messageInterceptor)
        {
            _cartBuilderFactory = cartBuilderFactory;
            _conversationState = conversationState;
            _orderModule = orderModule;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ViewCartAsync,
                CreateOrderPromptAsync,
                CreateOrderOrBack
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ViewCartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfileAccessor = _conversationState.CreateProperty<UserProfile>(nameof(UserProfile));
            var userProfile = await userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            using (var cartBuilder = _cartBuilderFactory.Create(userProfile.Customer))
            {
                var cart = await cartBuilder.GetCartAsync();

                var cards = cart.Items.GetCards();

                await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(cards.Select(c => c.ToAttachment())), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Cart total: {cart.Total} {cart.Currency}"), cancellationToken);
            }

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> CreateOrderPromptAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you want to create order?"),
                Choices = new []
                {
                    new Choice { Value = CreateOrder },
                    new Choice { Value = BackToSearch } 
                }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> CreateOrderOrBack(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = ((FoundChoice) stepContext.Result)?.Value;

            if (string.Equals(result, CreateOrder, StringComparison.InvariantCultureIgnoreCase))
            {
                var userProfileAccessor = _conversationState.CreateProperty<UserProfile>(nameof(UserProfile));
                var userProfile = await userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                using (var cartBuilder = _cartBuilderFactory.Create(userProfile.Customer))
                {
                    var cart = await cartBuilder.GetCartAsync();
                    var order = await _orderModule.CreateOrderFromCartAsync(cart.Id, cancellationToken);

                    await cartBuilder.RemoveCartAsync();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Order created. Its number: {order.Number}{Environment.NewLine}Amount: {order.Total} {order.Currency}"), cancellationToken);
                }
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
