using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Quests
{
    /// <summary>
    /// Holds a group of quest conditions.
    /// </summary>
    public class CompoundCondition : QuestCondition
    {
        [SerializeField]
        private QuestConditionMode m_ConditionMode;

        [SerializeField]
        private int m_MinimumNumberOfConditions;

        [SerializeField]
        private List<QuestCondition> m_Conditions = new List<QuestCondition>();

        private int m_SucceededConditionsCount;


        [Zenject.Inject]
        private void Construct(DiContainer diContainer)
        {
            for(int i = 0; i < m_Conditions.Count; i++)
            {
                if (!ReferenceEquals(m_Conditions[i], null))
                    diContainer.Inject(m_Conditions[i]);
            }
        }

        public override void StartChecking(QuestConditionMetDelegate conditionMetCallback)
        {
            base.StartChecking(conditionMetCallback);

            for(int i = 0; i < m_Conditions.Count; i++)
            {
                if (!ReferenceEquals(m_Conditions[i], null))
                {
                    m_Conditions[i].Initialize(quest, questNode);
                    m_Conditions[i].StartChecking(OnQuestConditionMet);
                }
            }
        }
               

        public override void StopChecking()
        {
            base.StopChecking();
            for (int i = 0; i < m_Conditions.Count; i++)
            {
                if (!ReferenceEquals(m_Conditions[i], null))
                    m_Conditions[i].StopChecking();
            }
        }

        private void OnQuestConditionMet(QuestCondition questCondition)
        {
            m_SucceededConditionsCount++;
            if (areConditionsMet)
                SetTrue();
        }

        public bool areConditionsMet
        {
            get
            {
                if (m_Conditions == null || m_Conditions.Count == 0)
                    return true;

                if (m_SucceededConditionsCount == 0)
                    return false;

                switch (m_ConditionMode)
                {
                    case QuestConditionMode.All:
                        return m_SucceededConditionsCount >= m_Conditions.Count;
                    case QuestConditionMode.Any:
                        return m_SucceededConditionsCount > 0;
                    case QuestConditionMode.AtLeast:
                        return m_SucceededConditionsCount >= m_MinimumNumberOfConditions;
                }

                return false;
            }
        }
    }
}