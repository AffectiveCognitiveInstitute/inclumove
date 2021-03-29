using Aci.Unity.Events;
using Zenject;

namespace Aci.Unity.Gamification
{
    /// <summary>
    /// Basic interface for user metrics trackers.
    /// </summary>
    public interface IUserMetricsTracker : IAciEventHandler<UserArgs>
    {
        void UpdateUserData();
    }
}
