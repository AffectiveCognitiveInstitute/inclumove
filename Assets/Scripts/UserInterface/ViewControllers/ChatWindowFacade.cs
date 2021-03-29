using System;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class ChatWindowFacade : IChatWindowFacade
    {
        private IChatWindow m_ChatWindow;

        public void AddMessage(GameObject gameObject)
        {
            m_ChatWindow.AddMessage(gameObject);
        }

        public void Clear()
        {
            if (m_ChatWindow == null)
                throw new NullReferenceException("Missing reference to IChatWindow");

            m_ChatWindow.Clear();
        }

        public void Register(IChatWindow chatWindow)
        {
            if (chatWindow == null)
                throw new ArgumentNullException(nameof(chatWindow));

            if (m_ChatWindow != null)
                throw new InvalidOperationException("There is already an instance of IChatWindow registered");

            m_ChatWindow = chatWindow;
        }

        public void Unregister()
        {
            m_ChatWindow = null;
        }
    }
}