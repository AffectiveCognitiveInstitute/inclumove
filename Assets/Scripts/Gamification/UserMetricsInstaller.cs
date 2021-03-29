using Zenject;

namespace Aci.Unity.Gamification
{
    /// <summary>
    /// IOC Installer for user metrics.
    /// </summary>
    public class UserMetricsInstaller : MonoInstaller<UserMetricsInstaller>
    {
        /// <inheritoc/>
        public override void InstallBindings()
        {
            Container.Bind<WorkingHourTracker>().FromNew().AsSingle().NonLazy();

            Container.Bind<WorkingDayTracker>().FromNew().AsSingle().NonLazy();

            Container.Bind<BadgeTracker>().FromNew().AsSingle().NonLazy();
        }
    }
}
