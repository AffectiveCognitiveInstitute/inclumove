using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace BotConnector.Unity
{
    public interface IBot
    {
        /// <summary>
        /// Raised when the bot receives a message activity.
        /// </summary>
        ActivityEvent MessageReceived { get; }

        /// <summary>
        /// Raised when the bot receives an activity that is not a message.
        /// </summary>
        ActivityEvent SystemActivityReceived { get; }

        /// <summary>
        /// Raised when the connection to the bot server changed.
        /// </summary>
        ConnectionStatusEvent ConnectionStatusChanged { get; }

        /// <summary>
        /// Gets or sets the user to use for conversations.
        /// </summary>
        ChannelAccount Account { get; set; }

        /// <summary>
        /// Gets the ID of the current conversation.
        /// </summary>
        string ConversationId { get; }

        /// <summary>
        /// Gets or sets a way of communicating with the bot server.
        /// </summary>
        IConnectionStrategy Connection { get; set; }

        /// <summary>
        /// Gets or sets the storage.
        /// </summary>
        IBotStorage Storage { get; set; }

        /// <summary>
        /// Starts a new conversation. 
        /// </summary>
        /// <returns></returns>
        Task StartConversation();

        /// <summary>
        /// Ends the current conversation.
        /// </summary>
        /// <returns></returns>
        Task EndConversation();

        /// <summary>
        /// Sends an activity to the bot.
        /// </summary>
        /// <param name="activity">The activity to send</param>
        /// <returns></returns>
        Task SendActivityAsync(Activity activity);

        /// <summary>
        /// Gets or sets the handle. This is the ID for the bot.
        /// </summary>
        /// <remarks>
        /// Used to determine whether an activity comes from the bot.
        /// </remarks>
        string Handle { get; set; }

        /// <summary>
        /// Simulates the bot receiving an activity
        /// </summary>
        /// <param name="activity">The activity which should be received</param>
        void SimulateMessageReceived(Activity activity);        
    }

    [Serializable]
    public class ActivityEvent : UnityEvent<Activity> { }
}
