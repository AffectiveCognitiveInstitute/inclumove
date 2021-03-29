using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.Animation
{
    public interface IQueueableAnimation
    {
        Task Play();
    }
}