using Aci.Unity.Events;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public struct CounterValueChangedMessage
    {
        public string id;
        public int value;
    }

    public class MessageCounter : QuestCounter, IAciEventHandler<CounterValueChangedMessage>
    {
        [SerializeField, Tooltip("The id of CounterValueChangedMessage.")]
        private string m_MessageId;

        [SerializeField]
        private QuestCounterBehaviour m_Behaviour = QuestCounterBehaviour.Increment;

        private IAciEventManager m_EventManager;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager)
        {
            m_EventManager = eventManager;
        }

        public void OnEvent(CounterValueChangedMessage arg)
        {
            if (arg.id != m_MessageId)
                return;

            // Maybe this logic can go into the base class.
            switch (m_Behaviour)
            {
                case QuestCounterBehaviour.Increment:
                    value += arg.value;
                    break;
                case QuestCounterBehaviour.Replace:
                    value = arg.value;
                    break;
                case QuestCounterBehaviour.Decrement:
                    value -= arg.value;
                    break;
            }
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public override void StartListening()
        {
            RegisterForEvents();
        }

        public override void StopListening()
        {
            UnregisterFromEvents();
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }
    }


    public enum QuestCounterBehaviour
    {
        /// <summary>
        /// Increments the current value.
        /// </summary>
        Increment,

        /// <summary>
        /// Decrements the current value.
        /// </summary>
        Decrement,

        /// <summary>
        /// Replaces the current value.
        /// </summary>
        Replace
    }
}