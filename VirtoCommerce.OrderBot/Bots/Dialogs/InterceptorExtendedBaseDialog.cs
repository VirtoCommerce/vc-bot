using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector;

namespace VirtoCommerce.OrderBot.Bots.Dialogs
{
    public abstract class InterceptorExtendedBaseDialog : ComponentDialog
    {
        private readonly IMessageInterceptor _messageInterceptor;

        protected InterceptorExtendedBaseDialog(string dialogId, IMessageInterceptor messageInterceptor)
            : base(dialogId)
        {
            _messageInterceptor = messageInterceptor;
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = new CancellationToken())
        {
            var message = innerDc.Context.Activity.Text;

            var dialogInstance = innerDc.Stack.FirstOrDefault();

            bool HaveStateAndNotContainsDialogs(DialogInstance instance)
            {
                return instance?.State != null && !instance.State.ContainsKey("dialogs");
            }
            
            if (HaveStateAndNotContainsDialogs(dialogInstance))
            {
                var result = await _messageInterceptor.InterceptAsync(message.Trim(), innerDc, cancellationToken);

                if (result != null)
                {
                    return result;
                }
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }
    }
}
