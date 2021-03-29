using Aci.Unity.Bot;
using Aci.Unity.Events;

namespace Aci.Unity.Commands
{
    public class IgnoreAdaptivityLevelCommand : ReplyCommand
    {
        private readonly IAciEventManager m_AciEventManager;

        public IgnoreAdaptivityLevelCommand(IBotMessenger bot,
                                            IAciEventManager aciEventManager) : base(bot)
        {
            m_AciEventManager = aciEventManager;
        }

        public override string Type => "ignoreAdaptivity";

        public override void Execute(object param = null)
        {
            base.Execute(param);
            m_AciEventManager.Invoke(new AdaptivityLevelChangeRepliedArgs() { wasChanged = false });
        }
    }
}
