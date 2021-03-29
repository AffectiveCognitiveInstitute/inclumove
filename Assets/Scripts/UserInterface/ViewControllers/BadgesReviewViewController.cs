using Aci.UI.Binding;
using System;
using System.Collections.Generic;

namespace Aci.Unity.UserInterface
{
    public class BadgesReviewViewController : MonoBindable
    {
        private int m_Tier1Badges;
        public int tier1Badges
        {
            get => m_Tier1Badges;
            set => SetProperty(ref m_Tier1Badges, value);
        }

        private int m_Tier2Badges;
        public int tier2Badges
        {
            get => m_Tier2Badges;
            set => SetProperty(ref m_Tier2Badges, value);
        }

        private int m_Tier3Badges;
        public int tier3Badges
        {
            get => m_Tier3Badges;
            set => SetProperty(ref m_Tier3Badges, value);
        }

        private int m_TotalBadges;
        public int totalBadges
        {
            get => m_TotalBadges;
            set => SetProperty(ref m_TotalBadges, value);
        }

        private Queue<Action> m_Actions = new Queue<Action>();
        private Action m_Callback;

        public void Set(int tier1, int tier2, int tier3, Action callback = null)
        {
            m_Callback = callback;
            if (tier1 > 0)
            {
                m_Actions.Enqueue(() => tier1Badges = tier1);
                m_Actions.Enqueue(() => totalBadges += tier1Badges);
            }

            if (tier2 > 0)
            {
                m_Actions.Enqueue(() => tier2Badges = tier2);
                m_Actions.Enqueue(() => totalBadges += tier2Badges * 2);
            }

            if (tier3 > 0)
            {
                m_Actions.Enqueue(() => tier3Badges = tier3);
                m_Actions.Enqueue(() => totalBadges += tier3Badges * 5);
            }

            Execute();
        }

        private void Execute()
        {
            if (m_Actions.Count == 0)
            {
                m_Callback?.Invoke();
                return;
            }

            Action action = m_Actions.Dequeue();
            action();
        }

        public void OnNumberScrollerCompleted(UINumberScroller numberScroller)
        {
            Execute();
        }
    }
}