using Microsoft.Bot.Connector.DirectLine;
using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Default settings for rendering
    /// </summary>
    [Serializable]
    public class RenderSettings : MonoBehaviour, IRenderSettings
    {
        #region Fields

        [SerializeField]
        private bool addMissingRenderer = false;

        [SerializeField]
        private Theme theme;

        [SerializeField]
        private CardActionHandler[] cardActionHandlers;

        #endregion

        #region Properties

        public bool AddMissingRenderer
        {
            get { return addMissingRenderer; }
            set { addMissingRenderer = value; }
        }

        public Theme Theme
        {
            get { return theme; }
            set { theme = value; }
        }

        public IDictionary<string, CardActionHandler> CardActionHandlers
        {
            get;
            set;
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            SetupActionHandlers();
        }

        #endregion

        private void SetupActionHandlers()
        {
            if (cardActionHandlers.Any(handler => string.IsNullOrEmpty(handler.Type)))
                Debug.LogWarning("There are invalid type fields for card action handlers in the chat configuration.");

            if (cardActionHandlers.Any(handler => handler.Handler == null))
                Debug.LogWarning("There are invalid handler fields for card action handlers in the chat configuration.");

            CardActionHandlers = cardActionHandlers
                .Where(handler => !string.IsNullOrEmpty(handler.Type))
                .ToDictionary(handler => handler.Type);
        }

        private bool GetHandlerForAction(CardAction cardAction)
        {
            if (cardAction.Type != null && CardActionHandlers.ContainsKey(cardAction.Type))
            {
                CardActionHandlers[cardAction.Type].Handler.Invoke(cardAction, GetComponent<IChat>());
                return true;
            }

            return false;
        }

        #region Methods

        /// <inheritdoc/>
        public virtual IRenderContext CreateContextForBot()
        {
            return new RenderContext(this, Promise.Resolved(), Theme.BotActivity, GetHandlerForAction);
        }

        /// <inheritdoc/>
        public virtual IRenderContext CreateContextForUser(IPromise responseStatus)
        {
            return new RenderContext(this, responseStatus, Theme.UserActivity, GetHandlerForAction);
        }

        /// <inheritdoc/>
        public virtual Type ResolveCustomRenderer<T>(T content)
        {
            if (content is string)
                return typeof(TextRenderer);
            if (content is DateTime)
                return typeof(TimestampRenderer);

            return null;
        }

        #endregion
    }
}
