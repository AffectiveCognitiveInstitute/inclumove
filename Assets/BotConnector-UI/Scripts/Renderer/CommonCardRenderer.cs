using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a <see cref="ICommonCard"/>
    /// </summary>
    public abstract class CommonCardRenderer : RendererBase<Attachment>
    {
        /// <summary>
        /// The title of the card.
        /// </summary>
        public GameObject Title;

        /// <summary>
        /// The subtitle of the card.
        /// </summary>
        public GameObject Subtitle;

        /// <summary>
        /// The text of the card.
        /// </summary>
        public GameObject Text;

        /// <summary>
        /// The container for buttons.
        /// </summary>
        public GameObject CardActionPanel;

        /// <inheritdoc/>
        public override void Render(Attachment attachment, IRenderContext context)
        {

            //RenderOrSetInactive(Title, card.Title, context);
            //RenderOrSetInactive(Subtitle, card.Subtitle, context);
            //RenderOrSetInactive(Text, card.Text, context);

            //card.Buttons.ForEach(action =>
            //{
            //    var button = Instantiate(context.Theme.Button).GetComponent<Button>();
            //    context.RenderToTarget(button.gameObject, action.Title);
            //    button.onClick.AddListener(() => context.InvokeCardAction(action));
            //    button.transform.SetParent(CardActionPanel.transform);
            //});
        }

        private void RenderOrSetInactive(GameObject target, string content, IRenderContext context)
        {
            if (target == null)
                return;
            else if (content == null)
                target.SetActive(false);
            else
                context.RenderToTarget(target, content);
        }
    }
}
