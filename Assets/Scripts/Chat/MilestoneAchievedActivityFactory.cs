using Aci.Unity.Data;
using Aci.Unity.UI.Localization;
using Aci.Unity.UserInterface.Factories;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Chat
{
    public class MilestoneAchievedActivityFactory : PlaceholderFactory<MilestoneData, Activity>
    {
        private ILocalizationManager m_LocalizationManager;

        [Zenject.Inject]
        public MilestoneAchievedActivityFactory(ILocalizationManager localizationManager)
        {
            m_LocalizationManager = localizationManager;
        }

        public override Activity Create(MilestoneData milestone)
        {
            Activity activity = new Activity();

            try
            {
                activity.Id = Guid.NewGuid().ToString();
                activity.Type = "message"; 
                activity.Text = m_LocalizationManager.GetLocalized($"{{{{milestone_achieved}}}}");
                activity.Timestamp = DateTime.UtcNow;


                activity.Attachments = new List<Attachment>()
                {
                    new Attachment(AttachmentContentType.MilestoneAchievement, null, milestone) 
                };

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return activity;
        }
    }
}
