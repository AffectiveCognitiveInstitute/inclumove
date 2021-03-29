using Aci.Unity.Events;
using BotConnector.Unity;
using System;
using UnityEngine;

namespace Aci.Unity.Chat
{
    public class UserLoginEventHandler : MonoBehaviour, IAciEventHandler<UserLoginArgs>
    {
        private IAciEventManager m_EventManager;
        private IBot m_Bot;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager, IBot bot)
        {
            m_EventManager = eventManager;
            m_Bot = bot;
        }

        private void OnEnable()
        {
            RegisterForEvents();
        }

        private void OnDisable()
        {
            UnregisterFromEvents();
        }

        async void IAciEventHandler<UserLoginArgs>.OnEvent(UserLoginArgs arg)
        {
            if (!arg.success)
                return;

            await m_Bot.EndConversation();
            m_Bot.Account.Id = GenerateUniqueId();
            await m_Bot.StartConversation();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        private string GenerateUniqueId()
        {
            var epoch = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            var timestamp = (DateTime.UtcNow - epoch).TotalSeconds;
            var random = new System.Random();

            return string.Format("{0:X}{1:X}", Convert.ToInt32(timestamp), random.Next(1000000000));
        }
    }
}
