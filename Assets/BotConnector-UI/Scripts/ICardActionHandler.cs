using Microsoft.Bot.Connector.DirectLine;
using System;
using UnityEngine.Events;

namespace BotConnector.Unity.UI
{
    public interface ICardActionHandler
    {
        void HandleAction(CardAction cardAction, IChat bot);
    }

    [Serializable]
    public class CardActionEvent : UnityEvent<CardAction, IChat>
    {
    }

    [Serializable]
    public class CardActionHandler
    {
        public string Type;
        public CardActionEvent Handler;
    }
}
