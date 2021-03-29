namespace Aci.Unity.Commands
{
    public abstract class SuggestedActionCommand : ICommand
    {
        public abstract string Type { get; }
        public abstract void Execute(object param = null);
    }
}
