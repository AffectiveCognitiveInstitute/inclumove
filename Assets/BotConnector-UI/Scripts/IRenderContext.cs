using Microsoft.Bot.Connector.DirectLine;
using RSG;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    public interface IRenderContext
    {
        /// <summary>
        /// Convenience helper for downloading an image as sprite.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <returns></returns>
        IPromise<Sprite> GetSpriteFromUri(string uri);

        /// <summary>
        /// Invokes an action based on a card action. 
        /// To validate see <see cref="IsSupportedAction(CardAction)"/>.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        bool InvokeCardAction(CardAction action);

        /// <summary>
        /// Renders content for a target.
        /// </summary>
        /// <typeparam name="T">The content type.</typeparam>
        /// <param name="target">The render target.</param>
        /// <param name="content">The content to render.</param>
        void RenderToTarget<T>(GameObject target, T content);

        /// <summary>
        /// Renders content.
        /// </summary>
        /// <typeparam name="T">The content type.</typeparam>
        /// <param name="content">The content to render.</param>
        /// <returns>The render result.</returns>
        GameObject Render<T>(T content);

        /// <summary>
        /// Gets the status of an activity, e.g. resolved if the activity was sent successfully.
        /// </summary>
        IPromise Status { get; }

        /// <summary>
        /// Gets the theme.
        /// </summary>
        Theme Theme { get; }
    }
}
