using Aci.Unity.Events;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;

namespace Aci.Unity.Chat
{
    /// <summary>
    /// Class reponsible for creating chat messages when a milestone has been achieved.
    /// </summary>
    public class MilestoneAchievedHandler : MonoBehaviour, IAciEventHandler<MilestoneAchievedArgs>
    {
        private IAciEventManager m_EventManager;
        private MilestoneAchievedActivityFactory m_Factory;
        private IBot m_Bot;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager, MilestoneAchievedActivityFactory factory, IBot bot)
        {
            m_EventManager = eventManager;
            m_Factory = factory;
            m_Bot = bot;
        }

        private void Awake()
        {
            RegisterForEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        public void OnEvent(MilestoneAchievedArgs arg)
        {
            Debug.Log("Received milestone achieved event!");
            Activity activity = m_Factory.Create(arg.milestone);
            m_Bot.SimulateMessageReceived(activity);
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<MilestoneAchievedArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<MilestoneAchievedArgs>(this);
        }
    }
}