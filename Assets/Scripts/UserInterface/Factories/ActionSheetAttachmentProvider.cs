using Aci.Unity.Commands;
using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.Factories
{
    /// <summary>
    /// Provider for cards with multiple options
    /// </summary>
    public class ActionSheetAttachmentProvider : AttachmentProviderBase
    {
        private MonoMemoryPool<ActionSheetCardViewController> m_Pool;
        private SuggestedActionCommandLibrary m_CommandLibrary;

        public override string type => AttachmentContentType.ActionSheet;

        [Zenject.Inject]
        private void Construct(MonoMemoryPool<ActionSheetCardViewController> pool,
                               SuggestedActionCommandLibrary commandLibrary)
        {
            m_Pool = pool;
            m_CommandLibrary = commandLibrary;
        }

        public override GameObject GetGameObject(Activity activity)
        {
            IList<CardAction> actions = activity.SuggestedActions.Actions;
            ActionSheetCardViewController vc = m_Pool.Spawn();
            List<ActionSheetOption> options = new List<ActionSheetOption>();
            foreach (var action in actions)
            {
                ICommand command = m_CommandLibrary.GetCommand(action.Type);
                options.Add(new ActionSheetOption(action.Image, action.Title, command));
            }
            vc.Initialize(options, activity.Text);
            return vc.gameObject;
        }
    }
}
