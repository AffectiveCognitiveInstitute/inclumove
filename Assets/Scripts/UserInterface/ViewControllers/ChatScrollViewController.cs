using Aci.Unity.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(ScrollRect))]
    public class ChatScrollViewController : MonoBehaviour,
                                            IAciEventHandler<BotMessageReceivedEvent>,
                                            IAciEventHandler<NewMessageNotificationClickedEvent>
    {
        [SerializeField]
        private AnimationCurve AutoScrollThreshold;

        [SerializeField]
        private float m_TimeToScroll = 0.3f;

        private ScrollRect m_ScrollRect;
        private bool m_IsDirty;
        private float m_StartPosition;
        private IAciEventManager m_AciEventManager;
        private float m_StartTime;
        private bool m_HasScrolledFirstTime;

        [Zenject.Inject]
        private void Construct(IAciEventManager aciEventManager)
        {
            m_AciEventManager = aciEventManager;
        }

        private void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
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
            // Wait until the scrollview's content becomes larger than the scrollview's viewport.
            if(m_ScrollRect.verticalNormalizedPosition == 1f && !m_HasScrolledFirstTime)
            {
                m_HasScrolledFirstTime = true;
                SetDirty();
                return;
            }

            // Auto scroll when the vertical normalized position is smaller than a defined threshold and when the user isn't currently
            // scrolling. 
            if (Input.mouseScrollDelta.y > 0.1f || m_ScrollRect.verticalNormalizedPosition > AutoScrollThreshold.Evaluate(m_ScrollRect.content.rect.height))
                return;

            SetDirty();
        }

        void IAciEventHandler<NewMessageNotificationClickedEvent>.OnEvent(NewMessageNotificationClickedEvent arg)
        {
            SetDirty();
        }

        private void SetDirty()
        {
            m_StartTime = Time.time;
            m_StartPosition = m_ScrollRect.verticalNormalizedPosition;
            m_IsDirty = true;
        }

        public void RegisterForEvents()
        {
            m_AciEventManager.AddHandler<BotMessageReceivedEvent>(this);
            m_AciEventManager.AddHandler<NewMessageNotificationClickedEvent>(this);
        }

        public void UnregisterFromEvents()
        {
            m_AciEventManager.RemoveHandler<BotMessageReceivedEvent>(this);
            m_AciEventManager.RemoveHandler<NewMessageNotificationClickedEvent>(this);
        }

        private void Update()
        {
            if (!m_IsDirty)
                return;

            float t = (Time.time - m_StartTime) / m_TimeToScroll;
            m_ScrollRect.verticalNormalizedPosition = Mathf.SmoothStep(m_StartPosition, 0, t);

            if (t >= 1)
                m_IsDirty = false;
        }
    }

    public struct BotMessageReceivedEvent { }
}
