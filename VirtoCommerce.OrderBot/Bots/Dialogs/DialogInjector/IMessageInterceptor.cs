using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector
{
    public interface IMessageInterceptor
    {
        Task<DialogTurnResult> InterceptAsync(string message, DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}
