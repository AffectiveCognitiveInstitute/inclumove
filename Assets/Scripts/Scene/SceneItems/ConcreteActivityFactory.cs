using Aci.Unity.Data.JsonModel;
using Aci.Unity.Models;
using Aci.Unity.UserInterface.Factories;
using Aci.Unity.Util;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Zenject;

namespace Aci.Unity.Scene
{
    public class ConcreteActivityFactory : IFactory<ActivityData, Activity>
    {
        private IConfigProvider m_ConfigProvider;

        [ConfigValue("workflowDirectory")]
        private string workflowDirectory { get; set; } = "";

        public ConcreteActivityFactory(IConfigProvider configProvider)
        {
            m_ConfigProvider = configProvider;
            m_ConfigProvider.RegisterClient(this);
            m_ConfigProvider.UnregisterClient(this);
        }

        public Activity Create(ActivityData param)
        {
            Activity activity = new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = "message",
                Text = param.message,
                Timestamp = DateTime.UtcNow
            };


            if (!string.IsNullOrWhiteSpace(param.mediaFilePath))
            {
                JContainer container = null;
                switch (param.contentType)
                {
                    case AttachmentContentType.Material:
                        MaterialCard matCard = new MaterialCard() { Materials = null, Image = new CardImage(Path.Combine(workflowDirectory, param.mediaFilePath)), Text = activity.Text };
                        container = JObject.FromObject(matCard);
                        break;
                    case AttachmentContentType.Video:
                        VideoCard videoCard = new VideoCard() { Text = activity.Text, Media = new List<MediaUrl>() { new MediaUrl(Path.Combine(workflowDirectory, param.mediaFilePath)) } };
                        container = JObject.FromObject(videoCard);
                        break;
                    default:
                        break;
                }

                activity.Attachments = new List<Attachment>()
                {
                    new Attachment(param.contentType, null, container)
                };
            }

            return activity;
        }
    }
}