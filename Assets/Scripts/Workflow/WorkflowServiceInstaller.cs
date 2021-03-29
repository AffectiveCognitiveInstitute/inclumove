using Aci.Unity.Gamification;
using Aci.Unity.Scene;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Aci.Unity.Workflow
{
    public class WorkflowServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IWorkflowService>().To<WorkflowService>().FromNew().AsSingle();
            Container.Bind<WorkflowLoader>().FromComponentInHierarchy().AsSingle();
        }
#if false
        public class MockBadgeServiceInstaller : Installer<MockBadgeServiceInstaller>
        {
            public override void InstallBindings()
            {
                IBadgeService badgeService = Substitute.For<IBadgeService>();
                badgeService.currentBadges.Returns(new Data.BadgeData()
                {
                    AmountBadges = new int[] { 3, 2, 1 },
                    StreakBadges = new int[] { 2, 1, 1 },
                    TimeBadges = new int[] { 5, 2, 1 }
                });

                IReadOnlyCollection<int> fastLevels = Array.AsReadOnly(new int[] { 2, 5, 10 });
                badgeService.fastLevels.Returns(fastLevels);

                IReadOnlyCollection<int> amountLevels = Array.AsReadOnly(new int[] { 4, 6, 8 });
                badgeService.amountLevels.Returns(amountLevels);

                IReadOnlyCollection<int> streakLevels = Array.AsReadOnly(new int[] { 1, 3, 5 });
                badgeService.streakLevels.Returns(streakLevels);

                badgeService.currentAmount.Returns(UnityEngine.Random.Range(0, amountLevels.Max() + 1));
                badgeService.currentFast.Returns(UnityEngine.Random.Range(0, fastLevels.Max() + 1));
                badgeService.currentStreak.Returns(UnityEngine.Random.Range(0, streakLevels.Max() + 1));

                Container.Bind<IBadgeService>().FromInstance(badgeService);
            }
        }        
#endif
    }
}