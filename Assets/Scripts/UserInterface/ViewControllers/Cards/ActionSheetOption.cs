using Aci.Unity.Commands;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public struct ActionSheetOption
    {
        public string message { get; }
        public ICommand command { get; }
        public string icon { get; }


        public ActionSheetOption(string icon, string message, ICommand command)
        {
            this.message = message;
            this.icon = icon;
            this.command = command;
        }

        public void Invoke()
        {
            command.Execute(this);
        }
    }
}
