using Aci.Unity.Data;
using Aci.Unity.Events;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public class UnlockRewardTrigger : QuestTrigger
    {
        [SerializeField]
        private string m_RewardId;

        private RewardCollection m_RewardCollection;

        [Zenject.Inject]
        private void Construct(RewardCollection rewardCollection)
        {
            m_RewardCollection = rewardCollection;
        }

        public override void Execute()
        {
            foreach(RewardData data in m_RewardCollection)
            {
                if (data.id != m_RewardId)
                    continue;
                data.unlocked = true;
            }
        }
    }
}