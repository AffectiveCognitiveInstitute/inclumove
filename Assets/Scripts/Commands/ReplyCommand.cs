using Aci.Unity.Bot;
using Aci.Unity.UserInterface.ViewControllers;
using BotConnector.Unity;

namespace Aci.Unity.Commands
{
    public class ReplyCommand : SuggestedActionCommand
    {
        private IBotMessenger m_BotMessenger;

        public override string Type => "reply";

        public ReplyCommand(IBotMessenger bot)
        {
            m_BotMessenger = bot;
        }

        public override void Execute(object param = null)
        {
            ActionSheetOption actionSheetOption = (ActionSheetOption)param;
            m_BotMessenger.SendMessageAsync(null, actionSheetOption.message);
        }
    }
}
