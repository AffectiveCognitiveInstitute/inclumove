using Microsoft.Bot.Connector.DirectLine;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a <see cref="HeroCard"/>.
    /// </summary>
    public class HeroCardRenderer : CommonCardRenderer
    {
        /// <summary>
        /// The hero image of the card.
        /// </summary>
        public Image HeroImage;

        /// <summary>
        /// Sprite to show if hero image is not available.
        /// </summary>
        public Sprite HeroImageNotAvailable;

        /// <inheritdoc/>
        public override void Render(Attachment attachment, IRenderContext context)
        {
            if (attachment == null)
                return;

            var card = attachment.Content as HeroCard;

            if (card.Images != null && card.Images.Any())
                context.GetSpriteFromUri(card.Images.First().Url)
                    .Then(sprite => HeroImage.sprite = sprite)
                    .Catch(error => HeroImage.sprite = HeroImageNotAvailable);
            else
                HeroImage.gameObject.SetActive(false);

            base.Render(attachment, context);
        }
    }
}
