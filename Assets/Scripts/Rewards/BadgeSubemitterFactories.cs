using Aci.Unity.UI.ViewControllers;

namespace Aci.Unity.Gamification.Rewards
{
    public struct BadgeSubemitterFactories
    {
        public BadgeParticleSystemViewController.Factory tier1Factory;
        public BadgeParticleSystemViewController.Factory tier2Factory;
        public BadgeParticleSystemViewController.Factory tier3Factory;

        public BadgeSubemitterFactories(BadgeParticleSystemViewController.Factory tier1Factory, BadgeParticleSystemViewController.Factory tier2Factory, BadgeParticleSystemViewController.Factory tier3Factory)
        {
            this.tier1Factory = tier1Factory;
            this.tier2Factory = tier2Factory;
            this.tier3Factory = tier3Factory;
        }
    }
}
