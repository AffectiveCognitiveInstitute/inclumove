using Microsoft.Bot.Connector.DirectLine;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a <see cref="ThumbnailCard"/>.
    /// </summary>
    public class ThumbnailRenderer : CommonCardRenderer
    {
        /// <summary>
        /// The thumbnail of the card.
        /// </summary>
        public Image Thumbnail;

        /// <summary>
        /// The sprite to show if the thumbnail is not available.
        /// </summary>
        public Sprite ThumbnailNotAvailable;


        public override void Render(Attachment attachment, IRenderContext context)
        {
            if (!(attachment.Content is ThumbnailCard))
                return;

            var card = attachment.Content as ThumbnailCard;

            if (card.Images != null && card.Images.Any())
                context.GetSpriteFromUri(card.Images.First().Url)
                    .Then(sprite => Thumbnail.sprite = sprite)
                    .Catch(error => Thumbnail.sprite = ThumbnailNotAvailable);
            else
                Thumbnail.gameObject.SetActive(false);

            base.Render(attachment, context);
        }
    }
}
