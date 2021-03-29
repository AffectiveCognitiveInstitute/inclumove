using Aci.Unity.Data;
using Aci.Unity.UserInterface.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class MilestoneFactoryInstaller : MonoInstaller<MilestoneFactoryInstaller>
    {
        [SerializeField]
        private Transform m_Parent;

        [SerializeField]
        private GameObject m_Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<MilestoneData, bool, MilestoneViewController, MilestoneViewController.Factory>().
                      FromComponentInNewPrefab(m_Prefab).UnderTransform(m_Parent);

        }
    }
}