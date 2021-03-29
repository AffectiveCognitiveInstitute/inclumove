namespace Aci.Unity.Gamification.Rewards
{
    public struct RewardParticleSystemFactoryParams
    {
        public SubEmitterParams amountBadgeParams;
        public SubEmitterParams speedBadgeParams;
        public SubEmitterParams streakBadgeParams;

        public RewardParticleSystemFactoryParams(SubEmitterParams amountBadgeParams, SubEmitterParams speedBadgeParams, SubEmitterParams streakBadgeParams)
        {
            this.amountBadgeParams = amountBadgeParams;
            this.speedBadgeParams = speedBadgeParams;
            this.streakBadgeParams = streakBadgeParams;
        }
    }
}
