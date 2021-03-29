using Microsoft.Bot.Connector.DirectLine;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.Factories
{
    /// <summary>
    /// Class reponsible for creating chat cards
    /// </summary>
    public class CardContentFactory : IFactory<Activity, GameObject>
    {
        private IAttachmentProviderRegistry m_AttachmentProvider;

        public CardContentFactory(IAttachmentProviderRegistry attachmentProvider)
        {
            m_AttachmentProvider = attachmentProvider;
        }

        /// <summary>
        /// Creates a card from the given <see cref="Activity"/>
        /// </summary>
        /// <param name="activity">The <see cref="Activity"/> from which the card should be created</param>
        /// <returns>Returns a <see cref="GameObject"/></returns>
        public GameObject Create(Activity activity)
        {
            IAttachmentProvider attachmentHandler;

            if (activity.SuggestedActions?.Actions?.Count > 0 && !string.IsNullOrWhiteSpace(activity.Text))
            {
                m_AttachmentProvider.TryGetProvider(AttachmentContentType.ActionSheet, out attachmentHandler);
            }
            else if (activity.Attachments?.Count > 0)
            {
                var attachment = activity.Attachments.FirstOrDefault();
                m_AttachmentProvider.TryGetProvider(attachment.ContentType, out attachmentHandler);
            }
            else if(!string.IsNullOrWhiteSpace(activity.Text))
            {
                m_AttachmentProvider.TryGetProvider(AttachmentContentType.Default, out attachmentHandler);
            }
            else
            {
                return null;
            }

            return attachmentHandler?.GetGameObject(activity);
        }  
    }
}
