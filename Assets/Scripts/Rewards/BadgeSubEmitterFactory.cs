using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification.Rewards
{
    public class BadgeSubEmitterFactory : IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>
    {
        public struct Result
        {
            public GameObject tier1SubemitterInstance;
            public GameObject tier2SubemitterInstance;
            public GameObject tier3SubemitterInstance;
        }

        private readonly BadgeSubemitterFactories m_Factories;

        [Zenject.Inject]
        public BadgeSubEmitterFactory(BadgeSubemitterFactories factories)
        {
            m_Factories = factories;
        }

        public Result Create(SubEmitterParams param)
        {
            Result result = new Result();

            if (param.particlesTier1Count > 0)
                result.tier1SubemitterInstance = m_Factories.tier1Factory.Create(param.particlesTier1Count).gameObject;

            if (param.particlesTier2Count > 0)
                result.tier1SubemitterInstance = m_Factories.tier2Factory.Create(param.particlesTier2Count).gameObject;

            if (param.particlesTier3Count > 0)
                result.tier1SubemitterInstance = m_Factories.tier3Factory.Create(param.particlesTier3Count).gameObject;

            return result;
        }
    }
}
