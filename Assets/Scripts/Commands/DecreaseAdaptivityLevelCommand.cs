﻿using Aci.Unity.Adaptivity;
using Aci.Unity.Bot;
using Aci.Unity.Events;

namespace Aci.Unity.Commands
{
    public class DecreaseAdaptivityLevelCommand : ReplyCommand
    {
        private readonly IAdaptivityService m_AdaptivityService;
        private readonly IAciEventManager m_AciEventManager;

        public DecreaseAdaptivityLevelCommand(IBotMessenger bot,
                                              IAdaptivityService adaptivityService,
                                              IAciEventManager aciEventManager) : base(bot)
        {
            m_AdaptivityService = adaptivityService;
            m_AciEventManager = aciEventManager;
        }

        public override string Type => "decreaseAdaptivity";

        public override void Execute(object param = null)
        {
            base.Execute(param);
            m_AdaptivityService.level--;
            m_AciEventManager.Invoke(new AdaptivityLevelChangeRepliedArgs() { wasChanged = true });
        }
    }
}
