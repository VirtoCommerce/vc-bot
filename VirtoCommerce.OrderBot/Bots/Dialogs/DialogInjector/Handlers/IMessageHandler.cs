using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector.Handlers
{
    public interface IMessageHandler
    {
        bool IsSuitableHandler(string message);
        Task<DialogTurnResult> HandleAsync(string message, DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}
