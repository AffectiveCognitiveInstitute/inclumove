using Aci.Unity.Data;
using Aci.Unity.UserInterface.ViewControllers;
using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class PreviousBadgesFactoryInstaller : MonoInstaller<PreviousBadgesFactoryInstaller>
    {
        [SerializeField]
        private GameObject m_Prefab;

        [SerializeField]
        private Transform m_Parent;

        public override void InstallBindings()
        {
            Container.BindFactory<DateTime, BadgeData, PreviousBadgeViewController, PreviousBadgeViewController.Factory>().
                      FromComponentInNewPrefab(m_Prefab).
                      UnderTransform(m_Parent);
        }
    }
}