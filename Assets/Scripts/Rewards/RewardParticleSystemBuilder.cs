using Zenject;

namespace Aci.Unity.Gamification.Rewards
{
    public class RewardParticleSystemBuilder
    {
        private readonly IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> m_AmountFactory;
        private readonly IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> m_SpeedFactory;
        private readonly IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> m_StreakFactory;

        public RewardParticleSystemBuilder(IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> amountFactory,
                                           IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> speedFactory,
                                           IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> streakFactory)
        {
            m_AmountFactory = amountFactory;
            m_SpeedFactory = speedFactory;
            m_StreakFactory = streakFactory;
        }

        public void Create(RewardParticleSystemFactoryParams parameters)
        {
            m_AmountFactory.Create(parameters.amountBadgeParams);
            m_SpeedFactory.Create(parameters.speedBadgeParams);
            m_StreakFactory.Create(parameters.streakBadgeParams);
        }
    } 
}
