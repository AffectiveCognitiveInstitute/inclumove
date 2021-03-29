using Aci.Unity.Bot;
using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.Factories
{
    public class VideoCardAttachmentProvider : AttachmentProviderBase
    {
        private VideoCardViewController.Factory m_Factory;

        public override string type => AttachmentContentType.Video;

        [Zenject.Inject]
        private void Construct(VideoCardViewController.Factory factory)
        {
            m_Factory = factory;
        }

        public override GameObject GetGameObject(Activity activity)
        {
            Attachment attachment = activity.Attachments.FirstOrDefault();
            VideoCard videoCard = attachment.GetRichCard<VideoCard>();
            VideoCardViewController vc = m_Factory.Create();
            

            vc.Initialize(videoCard.Text, videoCard.Media.FirstOrDefault()?.Url);
            return vc.gameObject;
        }
    }
}
