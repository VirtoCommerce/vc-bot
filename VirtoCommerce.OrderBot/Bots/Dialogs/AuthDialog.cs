using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Models;
using VirtoCommerce.OrderBot.Security;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public class AuthDialog : ComponentDialog
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ConversationState _conversationState;

        public AuthDialog(
            IAuthorizationService authService,
            ConversationState conversationState,
            MainDialog mainDialog
            ) 
            : base(nameof(AuthDialog))
        {
            _authorizationService = authService;
            _conversationState = conversationState;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(mainDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AuthPlease,
                TryToAuth
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AuthPlease(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var profileStateAccessor = _conversationState.CreateProperty<UserProfile>(nameof(UserProfile));
            var profile = await profileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            if (string.IsNullOrEmpty(profile.Customer.Id) && !await _authorizationService.IsAuthorizedAsync(profile.UserId))
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = stepContext.Context.Activity.CreateReply($"Your's identifier is: {profile.UserId}.{Environment.NewLine}Please send it to VC administrator."),
                    Choices = new[] { new Choice("Try auth") }
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }

            profile.Customer = await _authorizationService.GetCustomerAsync(profile.UserId);
            
            return await stepContext.BeginDialogAsync(nameof(MainDialog), cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> TryToAuth(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await stepContext.ReplaceDialogAsync(nameof(AuthDialog), cancellationToken: cancellationToken);
        }
    }
}
