using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public const string NewOrder = "New order";
        public const string TalkToSupport = "Talk to support";

        public MainDialog(CatalogDialog catalogDialog) 
            : base(nameof(MainDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(catalogDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                Init,
                Loop
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> Init(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("I'm a Virto Commerce order bot. What do you want to do?"),
                Choices = new[] {new Choice { Value = NewOrder }, new Choice { Value = TalkToSupport } }
            };
            
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> Loop(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = (stepContext.Result as FoundChoice)?.Value;

            switch (response)
            {
                case NewOrder:
                    return await stepContext.BeginDialogAsync(nameof(CatalogDialog), cancellationToken: cancellationToken);
                case TalkToSupport:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Coming soon"), cancellationToken);
                    break;
                default:
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog), cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
