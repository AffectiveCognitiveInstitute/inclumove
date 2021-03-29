using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.Animation
{
    public interface IAnimatedTransition
    {
        Task PlayEnterAsync();
        Task PlayExitAsync();
    }

}
