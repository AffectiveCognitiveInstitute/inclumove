using System;
using Aci.Unity.UI.Localization;
using Aci.Unity.UserInterface.Factories;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using Aci.Unity.Models;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Chat
{
    public class FeedbackActivityFactory : PlaceholderFactory<PredefinedActivity, Activity>
    {
        private string m_Domain;
        private IBot m_Bot;
        private ILocalizationManager m_LocalizationManager;

        [Zenject.Inject]
        private void Construct(ILocalizationManager localizationManager,
                               string domain)
        {
            m_LocalizationManager = localizationManager;
            m_Domain = "http://localhost:5001";
        }

        public override Activity Create(PredefinedActivity p)
        {
            Activity activity = new Activity();

            try
            {
                activity.Id = Guid.NewGuid().ToString();
                activity.Type = "message";
                activity.Text = m_LocalizationManager.GetLocalized($"{{{{{p.message}}}}}");
                activity.Timestamp = DateTime.UtcNow;
                if(p.hasImage)
                {
                    string path = m_Domain + "/" + p.image;
                    MaterialCard matCard = new MaterialCard() { Materials = null, Image = new CardImage(path), Text = activity.Text};
                    JContainer cont = JObject.FromObject(matCard);

                    activity.Attachments = new List<Attachment>()
                    {
                        new Attachment(AttachmentContentType.Material, null, cont)
                    }; 
                }

                if(p.suggestedActions != null && p.suggestedActions.Length > 0)
                {
                    List<CardAction> actions = new List<CardAction>();
                    for(int i = 0; i < p.suggestedActions.Length; i++)
                    {
                        SuggestedAction a = p.suggestedActions[i];
                        string localizedMessage = m_LocalizationManager.GetLocalized($"{{{{{a.Message}}}}}");
                        actions.Add(new CardAction(a.Type, localizedMessage, null, localizedMessage));
                    }

                    activity.SuggestedActions = new SuggestedActions()
                    {
                        Actions = actions
                    };
                }

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return activity;
        }
    }
}
