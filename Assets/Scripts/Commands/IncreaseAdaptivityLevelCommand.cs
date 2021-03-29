using Aci.Unity.Adaptivity;
using Aci.Unity.Bot;
using Aci.Unity.Events;

namespace Aci.Unity.Commands
{
    public class IncreaseAdaptivityLevelCommand : ReplyCommand
    {
        private readonly IAdaptivityService m_AdaptivityService;
        private readonly IAciEventManager m_AciEventManager;

        public IncreaseAdaptivityLevelCommand(IBotMessenger bot,
                                              IAdaptivityService adaptivityService, 
                                              IAciEventManager aciEventManager) : base(bot)
        {
            m_AdaptivityService = adaptivityService;
            m_AciEventManager = aciEventManager;
        }

        public override string Type => "increaseAdaptivity";

        public override void Execute(object param = null)
        {
            base.Execute(param);
            m_AdaptivityService.level++;
            m_AciEventManager.Invoke(new AdaptivityLevelChangeRepliedArgs() { wasChanged = true });
        }
    }
}
