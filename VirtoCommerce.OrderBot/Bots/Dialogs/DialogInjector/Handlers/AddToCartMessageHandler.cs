using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Infrastructure;

namespace VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector.Handlers
{
    public class AddToCartMessageHandler : IMessageHandler
    {
        private readonly AddToCartDialog _dialog;

        public AddToCartMessageHandler(AddToCartDialog dialog)
        {
            _dialog = dialog;
        }

        public async Task<DialogTurnResult> HandleAsync(string message, DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialog = dialogContext.Dialogs.Find(_dialog.GetType().Name);

            if (dialog == null)
            {
                dialogContext.Dialogs.Add(_dialog);
            }
            
            return await dialogContext.BeginDialogAsync(_dialog.GetType().Name, message.Substring(BotCommands.AddToCart.Length), cancellationToken);
        }

        public bool IsSuitableHandler(string message)
        {
            return message.StartsWith(BotCommands.AddToCart, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
