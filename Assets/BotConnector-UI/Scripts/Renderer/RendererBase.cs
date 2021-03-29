using UnityEngine;

namespace BotConnector.Unity.UI
{

    /// <summary>
    /// RendererBase is the base class for all renderer components.
    /// </summary>
    /// <typeparam name="T">The type of the content.</typeparam>
    public abstract class RendererBase<T> : MonoBehaviour, IRenderer
    {
        /// <inheritdoc/>
        public void Render(object content, IRenderContext context)
        {
            if (content is T)
                Render((T)content, context);
        }

        /// <summary>
        /// Renders the content for the given context.
        /// </summary>
        /// <param name="content">The content to render</param>
        /// <param name="context">The context</param>
        public abstract void Render(T content, IRenderContext context);
    }

    /// <summary>
    /// A component that renders specfic content.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Renders the content for the given context
        /// </summary>
        /// <param name="content">The content to render</param>
        /// <param name="context">The context</param>
        void Render(object content, IRenderContext context);
    }

}

