using Aci.Unity.Events;
using Aci.Unity.UI.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class NewMessageNotificationViewController : MonoBehaviour, IAciEventHandler<BotMessageReceivedEvent>
    {
        private const float MessageThreshold = 0.05f;
        [SerializeField]
        private Tweener m_Tweener;
        [SerializeField]
        private ScrollRect m_ScrollRect;
        private bool m_CanScroll;
        private IAciEventManager m_EventManager;
        private float m_LastMessageTime;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager)
        {
            m_EventManager = eventManager;
        }

        private void Awake()
        {
            m_Tweener.Seek(0);
        }

        private void OnEnable()
        {
            RegisterForEvents();
        }

        private void OnDisable()
        {
            UnregisterFromEvents();
        }

        void IAciEventHandler<BotMessageReceivedEvent>.OnEvent(BotMessageReceivedEvent arg)
        {

            // The vertical normalized position is always at 1 until the scroll content becomes
            // larger than the viewport. We always skip any checks until this occurs. 
            if (Mathf.Approximately(m_ScrollRect.verticalNormalizedPosition, 1f))
            {
                m_LastMessageTime = Time.time;
                return;
            }

            if (!m_CanScroll && m_ScrollRect.verticalNormalizedPosition >= MessageThreshold)
            {
                m_CanScroll = true;
            }
            // Only play if not currently playing, the scroll position is larger than a defined threshold,
            // and also check against previous message time. This prevents the message sometimes appearing in
            // unwanted scenarios
            if (!m_Tweener.isPlaying && m_ScrollRect.verticalNormalizedPosition >= MessageThreshold
                     && m_CanScroll
                     && Time.time - m_LastMessageTime > 0.1f)
            {
                m_Tweener.PlayForwards(false);
            }

            m_LastMessageTime = Time.time;

        }

        public void OnButtonClicked()
        {
            m_EventManager.Invoke(new NewMessageNotificationClickedEvent());
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }
    }

    public struct NewMessageNotificationClickedEvent { }
}
