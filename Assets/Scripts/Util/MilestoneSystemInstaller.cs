using Aci.Unity.Data;
using Aci.Unity.UserInterface.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class MilestoneSystemInstaller : MonoInstaller<MilestoneSystemInstaller>
    {
        [SerializeField]
        private MilestoneDataCollection m_Milestones;

        [SerializeField]
        private GameObject m_MilestoneAchievedPopupPrefab;

        [SerializeField]
        private Transform m_PopupTransform;

        public override void InstallBindings()
        {
            Container.Bind<MilestoneDataCollection>().FromInstance(m_Milestones).AsCached();
            Container.Bind<IMilestoneFacade>().To<MilestoneFacade>().AsCached();
            Container.BindFactory<MilestoneData, MilestoneAchievedViewController, MilestoneAchievedViewController.Factory>().
                      FromComponentInNewPrefab(m_MilestoneAchievedPopupPrefab).
                      UnderTransform(m_PopupTransform);
        }
    }
}