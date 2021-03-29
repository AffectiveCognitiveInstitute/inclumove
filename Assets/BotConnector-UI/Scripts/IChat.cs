using Microsoft.Bot.Connector.DirectLine;
using RSG;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// A chat for a bot.
    /// </summary>
    public interface IChat
    {
        /// <summary>
        /// Gets or sets the bot to use for the chat.
        /// </summary>
        IBot Bot { get; set; }

        /// <summary>
        /// Gets or sets the settings for rendering.
        /// </summary>
        IRenderSettings Settings { get; set; }

        /// <summary>
        /// Sends a message to the bot that gets rendered in the chat.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sending messages directly through the bot does not render them in the chat.
        /// You could set <see cref="Bot.OnlyBotMessages"/> to false, but messages would be
        /// rendered with a little delay depending on your internet connection.
        /// </remarks>
        IPromise<ResourceResponse> SendToBot(Activity message);
    }
}
