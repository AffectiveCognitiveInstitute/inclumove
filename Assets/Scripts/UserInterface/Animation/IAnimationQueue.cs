using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.Animation
{
    public interface IAnimationQueue
    {
        Task Play();

        Task PlaySafe();

        void Enqueue(IQueueableAnimation animation);

        void Clear();
    }
}