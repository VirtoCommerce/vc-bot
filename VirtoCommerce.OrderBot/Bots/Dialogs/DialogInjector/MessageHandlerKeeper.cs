using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector.Handlers;

namespace VirtoCommerce.OrderBot.Bots.Dialogs.DialogInjector
{
    public class MessageHandlerKeeper : IMessageHandlerStorage, IMessageHandlerReceiver
    {
        private readonly ICollection<IMessageHandler> _messageHandlerCollection = new List<IMessageHandler>();

        public IMessageHandlerStorage AddHandler(IMessageHandler messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            if (!_messageHandlerCollection.Contains(messageHandler))
            {
                _messageHandlerCollection.Add(messageHandler);
            }

            return this;
        }

        public IMessageHandler GetHandler(string message)
        {
            return _messageHandlerCollection.FirstOrDefault(h => h.IsSuitableHandler(message));
        }
    }
}
