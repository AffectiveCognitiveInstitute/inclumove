using Aci.Unity.Chat;
using Aci.Unity.Data;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.Factories
{
    public class MilestoneCardAttachmentProvider : AttachmentProviderBase
    {
        private MilestoneAchievedChatViewController.Factory m_Factory;

        public override string type => AttachmentContentType.MilestoneAchievement;

        [Zenject.Inject]
        private void Construct(MilestoneAchievedChatViewController.Factory factory)
        {
            m_Factory = factory;
        }

        public override GameObject GetGameObject(Activity activity)
        {
            return m_Factory.Create(activity.Text, activity.Attachments.FirstOrDefault().Content as MilestoneData).gameObject;
        }
    }
}