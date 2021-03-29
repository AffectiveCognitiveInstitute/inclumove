using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Linq;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders an <see cref="Activity"/>
    /// </summary>
    public class ActivityRenderer : RendererBase<Activity>
    {

        public bool WrapAttachment;
        public bool ShowStatusInfo;

        public GameObject WrapPanel;
        public GameObject TextPanel;
        public GameObject StatusPanel;

        /// <inheritdoc/>
        public override void Render(Activity activity, IRenderContext context)
        {
            // plain text

            if (!string.IsNullOrEmpty(activity.Text))
                context.RenderToTarget(TextPanel, activity.Text);
            else
                TextPanel.SetActive(false);

            // attachments

            if (activity.Attachments != null && activity.Attachments.Any())
                RenderAttachments(activity, context);

            // status and timestamp

            if (ShowStatusInfo)
            {
                context.RenderToTarget(StatusPanel, "Sending");

                context.Status
                    .Then(() => context.RenderToTarget(StatusPanel, activity.Timestamp ?? DateTime.Now))
                    .Catch(e => context.RenderToTarget(StatusPanel, "Could not send."));
            }
        }

        private void RenderAttachments(Activity activity, IRenderContext context)
        {
            GameObject parent = GetParent();

            if (activity.Attachments.Count > 1)
            {
                // render carousel
                var carousel = Instantiate(context.Theme.Carousel, parent.transform);
                activity.Attachments.ForEach(attachment => RenderAttachment(context, attachment, carousel.transform));
            }
            else
            {
                // render single card
                RenderAttachment(context, activity.Attachments.First(), parent.transform);
            }
        }

        private GameObject GetParent()
        {
            if (WrapAttachment)
            {
                WrapPanel.SetActive(true);
                return WrapPanel;
            }

            return gameObject;
        }

        private void RenderAttachment(IRenderContext context, Attachment attachment, Transform parent)
        {
            var card = context.Render(attachment);
            card?.transform.SetParent(parent);
            card?.transform.SetSiblingIndex(1);
        }
    }
}

