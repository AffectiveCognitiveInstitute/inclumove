using Microsoft.Bot.Connector.DirectLine;
using System;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Implements common card action handlers.
    /// </summary>
    public class CommonHandler : MonoBehaviour
    {
        /// <summary>
        /// Handles an action to open an url.
        /// </summary>
        /// <param name="cardAction">The action.</param>
        /// <param name="bot">The bot.</param>
        public void HandleOpenUrlAction(CardAction cardAction, IChat bot)
        {
            OnAction(cardAction, Application.OpenURL);
        }

        /// <summary>
        /// Handles an action to send back a message to the bot.
        /// </summary>
        /// <param name="cardAction">The action.</param>
        /// <param name="bot">The bot.</param>
        public void HandleSubmitAction(CardAction cardAction, IChat chat)
        {
            OnAction(cardAction, value => chat.SendToBot(new Activity(text: value)));
        }

        private bool OnAction(CardAction action, Action<string> onValidated)
        {
            string actionValue = action.Value as string;
            if (actionValue == null)
                return false;
            onValidated(actionValue);
            return true;
        }
    }

}

