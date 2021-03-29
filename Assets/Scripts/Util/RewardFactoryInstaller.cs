using Aci.Unity.Data;
using Aci.Unity.UserInterface;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class RewardFactoryInstaller : MonoInstaller<RewardFactoryInstaller>
    {
        [SerializeField]
        private Transform m_Parent;

        [SerializeField]
        private GameObject m_Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<RewardData, RewardViewController, RewardViewController.Factory>().
                      FromComponentInNewPrefab(m_Prefab).UnderTransform(m_Parent);
        }
    }
}