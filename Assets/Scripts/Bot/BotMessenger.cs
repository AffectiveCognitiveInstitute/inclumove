using Aci.Unity.Events;
using Aci.Unity.UI.Localization;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Threading.Tasks;
using Zenject;

namespace Aci.Unity.Bot
{
    /// <summary>
    /// Interface for sending messages and events to bot
    /// </summary>
    public class BotMessenger : IBotMessenger,
                                IDisposable,
                                IInitializable,
                                IAciEventHandler<LocalizationChangedArgs>
    {
        private IBot m_Bot;
        private ILocalizationManager m_LocalizationManager;
        private IAciEventManager m_AciEventManager;
        private string m_Locale;

        private Activity m_LastActivity = null;

        public BotMessenger(IBot bot, 
                            ILocalizationManager localizationManager,
                            IAciEventManager aciEventManager)
        {
            m_Bot = bot;
            m_LocalizationManager = localizationManager;
            m_AciEventManager = aciEventManager;
        }

        public void Dispose()
        {
            UnregisterFromEvents();
        }

        public void Initialize()
        {
            m_Locale = m_LocalizationManager.currentLocalization;
            RegisterForEvents();
        }

        public Task SendEventAsync(string name, object value)
        {
            /*m_LastActivity = new Activity
            {
                Type = "event",
                Name = name,
                Value = value,
                Locale = m_Locale
            };
            return m_Bot.SendActivityAsync(m_LastActivity);*/
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ResendLastActivityAsync()
        {
            if (m_LastActivity == null)
                return Task.CompletedTask;
            return m_Bot.SendActivityAsync(m_LastActivity);
        }

        public Task SendMessageAsync(string name, string message)
        {
            /*m_LastActivity = new Activity
            {
                Type = "message",
                Name = name,
                Text = message,
                Locale = m_Locale
            };
            return m_Bot.SendActivityAsync(m_LastActivity);*/
            return Task.CompletedTask;
        }

        public Task SendActivityAsync(Activity activity)
        {
            return m_Bot.SendActivityAsync(activity);
        }

        void IAciEventHandler<LocalizationChangedArgs>.OnEvent(LocalizationChangedArgs arg)
        {
            m_Locale = arg.ietf;
        }

        public void RegisterForEvents()
        {
            m_AciEventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_AciEventManager.RemoveHandler(this);
        }
    }
}
