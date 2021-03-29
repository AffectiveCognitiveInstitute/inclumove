using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Quests;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class MilestonesScreenViewController : MonoBehaviour
    {
        [SerializeField]
        private MilestoneDataCollection m_MilestoneCollection;

        private MilestoneViewController.Factory m_Factory;
        private IUserProfile m_UserProfile;
        private IQuestFacade m_QuestFacade;

        [Zenject.Inject]
        private void Construct(MilestoneViewController.Factory factory, IUserProfile userProfile, IQuestFacade questFacade)
        {
            m_Factory = factory;
            m_UserProfile = userProfile;
            m_QuestFacade = questFacade;
        }

        private void Awake()
        {
            Populate();
        }

        private void Populate()
        {
            foreach (MilestoneData data in m_MilestoneCollection)
            {
                if (data.requiredQuest != null)
                    m_Factory.Create(data, m_QuestFacade.GetQuestState(data.requiredQuest.id) == QuestState.Success);
                else
                    m_Factory.Create(data, false);
            }
        }
    }
}