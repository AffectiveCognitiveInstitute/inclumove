using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Data
{
    class RewardInstaller : MonoInstaller
    {
        [SerializeField]
        private RewardCollection m_RewardCollection;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RewardCollection>().FromInstance(m_RewardCollection.CreateCopy());
        }
    }
}
