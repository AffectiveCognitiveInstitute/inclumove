using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageAttachmentRenderer : RendererBase<Attachment>
    {
        public Image Image;

        private void Start()
        {
            Image = Image ?? GetComponent<Image>();
        }

        /// <inheritdoc/>
        public override void Render(Attachment attachment, IRenderContext context)
        {
            context.GetSpriteFromUri(attachment.ContentUrl)
                .Then(sprite => Image.sprite = sprite)
                .Catch(error => Debug.Log(error));
        }
    }
}


