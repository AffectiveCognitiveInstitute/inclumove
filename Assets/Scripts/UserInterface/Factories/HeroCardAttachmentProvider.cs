using Aci.Unity.Bot;
using Aci.Unity.Commands;
using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.Factories
{
    /// <summary>
    /// Provider for <see cref="HeroCard"/> attachments
    /// </summary>
    public class HeroCardAttachmentProvider : AttachmentProviderBase
    {
        /// <inheritdoc/>
        public override string type => AttachmentContentType.Hero;

        private MonoMemoryPool<HeroCardViewController> m_Pool;
        private SuggestedActionCommandLibrary m_CommandLibrary;
        private IBotMessenger m_BotMessenger;
        private IAttachmentProviderRegistry m_Registry;

        [Zenject.Inject]
        private void Construct(MonoMemoryPool<HeroCardViewController> pool,
                               SuggestedActionCommandLibrary commandLibrary)
        {
            m_Pool = pool;
            m_CommandLibrary = commandLibrary;
        }

        /// <inheritdoc/>
        override public GameObject GetGameObject(Activity activity)
        {
            var attachment = activity.Attachments.FirstOrDefault();
            var heroCard = attachment.GetRichCard<HeroCard>();
            var vc = m_Pool.Spawn();
            List<ActionSheetOption> actionSheetOptions = null;
            if (heroCard.Buttons != null)
            {
                actionSheetOptions = new List<ActionSheetOption>();
                foreach (var action in heroCard.Buttons)
                {
                    ICommand command = m_CommandLibrary.GetCommand(action.Type);
                    actionSheetOptions.Add(new ActionSheetOption(action.Image, action.Title, command));
                }
            }
            if (activity.SuggestedActions != null)
            {
                if(actionSheetOptions == null)
                    actionSheetOptions = new List<ActionSheetOption>();

                foreach (var action in activity.SuggestedActions?.Actions)
                {
                    ICommand command = m_CommandLibrary.GetCommand(action.Type);
                    actionSheetOptions.Add(new ActionSheetOption(action.Image, action.Title, command));
                }
            }

            vc.Initialize(heroCard.Text, heroCard.Images.FirstOrDefault()?.Url, actionSheetOptions);
            return vc.gameObject;
        }
    }
}
