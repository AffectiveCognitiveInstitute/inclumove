namespace Aci.Unity.Commands
{
    public interface ICommand
    {
        void Execute(object param = null);
    }
}