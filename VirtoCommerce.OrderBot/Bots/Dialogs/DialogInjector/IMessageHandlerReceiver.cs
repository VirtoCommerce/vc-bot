using VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector.Handlers;

namespace VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector
{
    public interface IMessageHandlerReceiver
    {
        IMessageHandler GetHandler(string message);
    }
}
