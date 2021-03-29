using System;
using Aci.Unity.Scene;
using Aci.Unity.UI.Localization;
using Aci.Unity.UserInterface.Factories;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using System.IO;
using Aci.Unity.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;
using Aci.Unity.Workflow;

namespace Aci.Unity.Chat
{
    public class IncorrectWorkflowStepActivityFactory : PlaceholderFactory<Activity>
    {
        private IncorrectWorkflowStepActivityLibrary m_ActivityLibrary;
        private string m_Domain;
        private IBot m_Bot;
        private IWorkflowService m_WorkflowService;
        private ILocalizationManager m_LocalizationManager;

        [Zenject.Inject]
        private void Construct(IWorkflowService workflowService, 
                               ILocalizationManager localizationManager,
                               IncorrectWorkflowStepActivityLibrary library,
                               string domain)
        {
            m_WorkflowService = workflowService;
            m_LocalizationManager = localizationManager;
            m_ActivityLibrary = library;
            m_Domain = "http://localhost:5001";
        }

        public override Activity Create()
        {
            Activity activity = new Activity();

            try
            {
                PredefinedActivity p = m_ActivityLibrary.Get(m_WorkflowService.currentStep);

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

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return activity;
        }
    }
}
