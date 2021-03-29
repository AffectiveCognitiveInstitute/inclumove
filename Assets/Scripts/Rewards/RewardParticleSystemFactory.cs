using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification.Rewards
{
    public class RewardParticleSystemFactory
    {
        private DiContainer m_DiContainer;

        public RewardParticleSystemFactory(DiContainer diContainer)
        {
            m_DiContainer = diContainer;
        }

        public void Create(string rewardId, RewardParticleSystemFactoryParams parameters)
        {
            RewardParticleSystemBuilder builder = m_DiContainer.TryResolveId<RewardParticleSystemBuilder>(rewardId);

            if(builder == null)
            {
                Debug.LogError($"Could not find a factory with reward id: {rewardId}");
                return;
            }

            builder.Create(parameters);
        }
    }
}
