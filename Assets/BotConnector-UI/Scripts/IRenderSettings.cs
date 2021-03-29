using RSG;
using System;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Settings for a <see cref="IRenderContext"/>.
    /// </summary>
    public interface IRenderSettings
    {
        /// <summary>
        /// Gets or sets whether to a renderer to a GameObject if its not present on that object.
        /// </summary>
        bool AddMissingRenderer { get; set; }

        /// <summary>
        /// Gets or sets the theme to use while rendering.
        /// </summary>
        Theme Theme { get; }

        /// <summary>
        /// Extension point for custom renderer.
        /// </summary>
        /// <typeparam name="T">The type of content to render.</typeparam>
        /// <param name="content">The actual content to render.</param>
        /// <returns>The type of the renderer that can handle the content type.</returns>
        Type ResolveCustomRenderer<T>(T content);

        /// <summary>
        /// Creates a valid render context for a user activity.
        /// </summary>
        /// <param name="responseStatus">The status for sending the activity.</param>
        /// <returns></returns>
        IRenderContext CreateContextForUser(IPromise responseStatus);

        /// <summary>
        /// Creates a valid render context for a bot activity.
        /// </summary>
        /// <returns></returns>
        IRenderContext CreateContextForBot();

    }
}
