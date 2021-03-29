using Microsoft.Bot.Connector.DirectLine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a <see cref="ReceiptCard"/>.
    /// </summary>
    public class ReceiptRenderer : RendererBase<Attachment>
    {
        /// <summary>
        /// The title of the card.
        /// </summary>
        public Text Title;

        /// <summary>
        /// Prefab for a visual separation of contents.
        /// </summary>
        public GameObject SeperatorPrefab;

        /// <summary>
        /// The panel for <see cref="Fact"/>s.
        /// </summary>
        public GameObject FactsPanel;

        /// <summary>
        /// The prefab for a <see cref="Fact"/>.
        /// </summary>
        public GameObject FactPrefab;

        /// <summary>
        /// The panel for <see cref="ReceiptItem"/>s.
        /// </summary>
        public GameObject ItemsPanel;

        /// <summary>
        /// The prefab for a <see cref="ReceiptItem"/>.
        /// </summary>
        public GameObject ItemPrefab;

        public override void Render(Attachment attachment, IRenderContext context)
        {
            var card = attachment.Content as ReceiptCard;

            Title.text = card.Title ?? string.Empty;

            card.Facts.ForEach(fact => RenderFact(fact, context));
            Separator();
            card.Items.ForEach(item => RenderItem(item, context));
            Separator();
        }

        private void Separator()
        {
            Instantiate(SeperatorPrefab);
        }

        private void RenderItem(ReceiptItem item, IRenderContext context)
        {
            if (item == null)
                return;

            var template = Instantiate(ItemPrefab);
            template.GetComponent<ReceiptItemRenderer>().Render(item, context);
        }

        private void RenderFact(Fact fact, IRenderContext context)
        {
            if (fact == null)
                return;

            var template = Instantiate(FactPrefab);
            template.GetComponent<FactRenderer>().Render(fact, context);
        }
    }

    public class FactRenderer : RendererBase<Fact>
    {
        public Text Key;
        public Text Value;

        public override void Render(Fact content, IRenderContext context)
        {
            Key.text = content.Key ?? string.Empty;
            Value.text = content.Value ?? string.Empty;
        }
    }

    public class ReceiptItemRenderer : RendererBase<ReceiptItem>
    {
        public Text Title;

        public Text Subtitle;

        public Text Text;

        public Text Price;

        public Text Quantity;

        public Image Image;

        public override void Render(ReceiptItem item, IRenderContext context)
        {
            Title.text = item.Title ?? string.Empty;
            Subtitle.text = item.Subtitle ?? string.Empty;
            Text.text = item.Text ?? string.Empty;
            Price.text = item.Price ?? string.Empty;
            Quantity.text = item.Quantity ?? string.Empty;

            if (item.Image != null)
            {
                context.GetSpriteFromUri(item.Image.Url)
                    .Then(sprite => Image.sprite = sprite);

                var router = Image.gameObject.AddComponent<ClickRouter>();
                router.OnClick.AddListener(() => context.InvokeCardAction(item.Image.Tap));

            }
        }
    }
}
