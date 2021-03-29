using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a <see cref="VideoCard"/>
    /// </summary>
    public class VideoCardRenderer : CommonCardRenderer
    {
        public Image Image;
        public VideoPlayer VideoPlayer;

        public override void Render(Attachment attachment, IRenderContext context)
        {
            if (!(attachment.Content is VideoCard))
                return;

            var card = attachment.Content as VideoCard;

            if(card.Image != null && card.Image.Url != null)
            {
                var router = Image.gameObject.AddComponent<ClickRouter>();
                router.OnClick.AddListener(Switch);

                context.GetSpriteFromUri(card.Image.Url)
                    .Then(sprite => Image.sprite = sprite);
            }

            if(card.Media != null && card.Media.Any())
            {
                SetupVideoPlayer(card);
            }
        }

        private void Switch()
        {
            throw new NotImplementedException();
        }

        private void SetupVideoPlayer(VideoCard card)
        {
            VideoPlayer.source = VideoSource.Url;
            VideoPlayer.url = card.Media.First().Url;
        }
    }
}
