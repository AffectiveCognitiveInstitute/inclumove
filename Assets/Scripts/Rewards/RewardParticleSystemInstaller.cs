using Aci.Unity.UI.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification.Rewards
{
    public class RewardParticleSystemInstaller : MonoInstaller<RewardParticleSystemInstaller>
    {
        [SerializeField]
        private RewardParticleSystems m_RewardParticleSystems;

        [SerializeField]
        private Transform m_Parent;

        public override void InstallBindings()
        {
            Container.DefaultParent = m_Parent;

            foreach(ParticleSystemData psd in m_RewardParticleSystems)
            {
                Container.Bind<RewardParticleSystemBuilder>().WithId(psd.reward.id).FromSubContainerResolve().
                          ByMethod(subContainer => RewardSubemitterFactoryInstaller.Install(subContainer, psd)).WithKernel().AsCached();
            }

            Container.Bind<RewardParticleSystemFactory>().ToSelf().AsCached();
        }

        public class RewardSubemitterFactoryInstaller : Installer<ParticleSystemData, RewardSubemitterFactoryInstaller>
        {
            private ParticleSystemData m_ParticleSystemData;

            [Zenject.Inject]
            public void Construct(ParticleSystemData psd)
            {
                m_ParticleSystemData = psd;
            }


            public override void InstallBindings()
            {
                Container.DefaultParent = Container.InheritedDefaultParent;

                // Amount Subemitters
                Container.Bind<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>().WithId("AmountFactory").FromSubContainerResolve().
                          ByMethod(subContainer => ConcreteBadgeSubemitterFactoryInstaller.Install(subContainer, m_ParticleSystemData.amountPrefabs)).
                          WithKernel().AsCached();

                // Speed Subemitters
                Container.Bind<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>().WithId("SpeedFactory").FromSubContainerResolve().
                          ByMethod(subContainer => ConcreteBadgeSubemitterFactoryInstaller.Install(subContainer, m_ParticleSystemData.speedPrefabs)).
                          WithKernel().AsCached();

                // Streak Subemitters
                Container.Bind<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>().WithId("StreakFactory").FromSubContainerResolve().
                          ByMethod(subContainer => ConcreteBadgeSubemitterFactoryInstaller.Install(subContainer, m_ParticleSystemData.streakPrefabs)).
                          WithKernel().AsCached();


                IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> amountFactory = Container.ResolveId<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>("AmountFactory");
                IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> speedFactory = Container.ResolveId<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>("SpeedFactory");
                IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result> streakFactory = Container.ResolveId<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>("StreakFactory");
                Container.Bind<RewardParticleSystemBuilder>().ToSelf().AsCached().WithArguments(amountFactory,
                                                                                                speedFactory,
                                                                                                streakFactory);
            }

            public class ConcreteBadgeSubemitterFactoryInstaller : Installer<BadgeSubemitterPrefabs, ConcreteBadgeSubemitterFactoryInstaller>
            {
                private BadgeSubemitterPrefabs m_Prefabs;

                [Zenject.Inject]
                public void Construct(BadgeSubemitterPrefabs prefabs)
                {
                    m_Prefabs = prefabs;
                }

                public override void InstallBindings()
                {
                    Container.DefaultParent = Container.InheritedDefaultParent;

                    Container.Bind<BadgeSubemitterFactories>().FromSubContainerResolve().ByMethod(subContainer => BadgeSubemitterFactoriesInstaller.Install(subContainer, m_Prefabs)).WithKernel().AsCached();
                    Container.Bind<IFactory<SubEmitterParams, BadgeSubEmitterFactory.Result>>().To<BadgeSubEmitterFactory>().AsCached();
                }
            }


            public class BadgeSubemitterFactoriesInstaller : Installer<BadgeSubemitterPrefabs, BadgeSubemitterFactoriesInstaller>
            {
                private readonly BadgeSubemitterPrefabs m_Prefabs;

                public BadgeSubemitterFactoriesInstaller(BadgeSubemitterPrefabs prefabs)
                {
                    m_Prefabs = prefabs;
                }

                public override void InstallBindings()
                {
                    Container.DefaultParent = Container.InheritedDefaultParent;

                    Container.BindFactory<int, BadgeParticleSystemViewController, BadgeParticleSystemViewController.Factory>().WithId("Tier1Factory").
                              FromComponentInNewPrefab(m_Prefabs.tier1Prefab).AsCached();
                    Container.BindFactory<int, BadgeParticleSystemViewController, BadgeParticleSystemViewController.Factory>().WithId("Tier2Factory").
                              FromComponentInNewPrefab(m_Prefabs.tier2Prefab).AsCached();
                    Container.BindFactory<int, BadgeParticleSystemViewController, BadgeParticleSystemViewController.Factory>().WithId("Tier3Factory").
                              FromComponentInNewPrefab(m_Prefabs.tier3Prefab).AsCached();
                    BadgeParticleSystemViewController.Factory tier1Factory = Container.ResolveId<BadgeParticleSystemViewController.Factory>("Tier1Factory");
                    BadgeParticleSystemViewController.Factory tier2Factory = Container.ResolveId<BadgeParticleSystemViewController.Factory>("Tier2Factory");
                    BadgeParticleSystemViewController.Factory tier3Factory = Container.ResolveId<BadgeParticleSystemViewController.Factory>("Tier3Factory");
                    Container.Bind<BadgeSubemitterFactories>().ToSelf().AsCached().WithArguments(tier1Factory,
                                                                                                 tier2Factory,
                                                                                                 tier3Factory);
                }
            }
        }
    }
}
