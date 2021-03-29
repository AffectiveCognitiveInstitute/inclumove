using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace BotConnector.Unity
{
    /// <summary>
    /// Connects to and disconnects from a conversation.
    /// </summary>
    public interface IConnectionStrategy
    {
        /// <summary>
        /// Connects to a conversation.
        /// </summary>
        /// <param name="conversation">The conversation to connect to.</param>
        /// <returns>A promsise indicating the outcome of connecting.</returns>
        Task ConnectAsync(Conversation conversation);

        /// <summary>
        /// Disconnects from a conversation.
        /// </summary>
        /// <returns>A promsie indicating the outcome of disconnecting.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Raised when a set of messages is received.
        /// </summary>
        event Action<ActivitySet> MessageReceived;
    }

    /// <summary>
    /// Establishes a connection over websocket.
    /// </summary>
    public sealed class WebsocketConnectionStrategy : IConnectionStrategy
    {
        #region Private fields

        private WebSocket socket;

        #endregion

        #region IConnectionStrategy members

        /// <inheritdoc/>
        public Task ConnectAsync(Conversation conversation)
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            if (conversation.StreamUrl == null)
                throw new ArgumentNullException(nameof(conversation.StreamUrl));

            Debug.Log("Attempting to connect to conversation over websocket");
            return ConnectInternal(conversation);
        }

        public Task DisconnectAsync()
        {
            socket?.Close();
            return Task.CompletedTask;
        }

        public event Action<ActivitySet> MessageReceived;

        #endregion

        #region Methods

        private Task ConnectInternal(Conversation conversation)
        {
            socket = new WebSocket(conversation.StreamUrl);
            socket.OnMessage += OnWebsocketMessage;
            socket.OnOpen += (_, __) =>
            {
                Debug.Log($"[BOT] Socket for conversation {conversation.ConversationId} is open.");
            };
            socket.OnError += (_, e) =>
            {
                Debug.LogException(e.Exception);
                Debug.Log($"[BOT] Socket for conversation {conversation.ConversationId} encountered exception. Message: {e.Message}");
            };
            socket.OnClose += (_, __) =>
            {
                Debug.Log($"Socket closed. Reason: {__.Reason}. Code: {__.Code}. WasClean: {__.WasClean}");
                Debug.Log($"[BOT] Socket for conversation {conversation.ConversationId} is closed.");
            };
            socket.Connect();

            return Task.CompletedTask;
        }


        private void OnWebsocketMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText || string.IsNullOrWhiteSpace(e.Data))
                return;

            try
            {
                var set = JsonConvert.DeserializeObject<ActivitySet>(e.Data);
                MessageReceived?.Invoke(set);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[BOT] Error deserializing JSON from socket. Message: {ex.Message}");
            }
        }

        #endregion

    }

    /// <summary>
    /// Establishes a connection over polling with HTTP.
    /// </summary>
    public sealed class PollingConnectionStrategy : IConnectionStrategy
    {
        #region Private fields

        private readonly IConversations api;
        private string conversationId;
        private int interval;
        private bool isPolling;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the seconds to wait between requests.
        /// </summary>
        /// <remarks>
        /// The value has to be greater than 0.
        /// </remarks>
        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value > 0 ? value : 1;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new strategy that uses polling.
        /// </summary>
        /// <param name="promiseTimer">The timer used to orchestrate polling.</param>
        /// <param name="interval">The polling interval in seconds.</param>
        public PollingConnectionStrategy(IConversations conversations, int interval = 1)
        {

            if (conversations == null)
                throw new ArgumentNullException(nameof(conversations));

            api = conversations;
            Interval = interval;
        }

        #endregion

        #region IConnectionStrategy members

        /// <inheritdoc/>
        public async Task ConnectAsync(Conversation conversation)
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            if (isPolling)
                await DisconnectAsync().ConfigureAwait(false);

            conversationId = conversation.ConversationId;
            isPolling = true;

#pragma warning disable
            Task.Run(() => Poll()).ConfigureAwait(false);
#pragma warning restore
        }

        /// <inheritdoc/>
        public Task DisconnectAsync()
        {
            isPolling = false;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public event Action<ActivitySet> MessageReceived;

        #endregion

        #region Private methods

        private async Task Poll()
        {
            string watermark = null;

            while (isPolling)
            {
                await Task.Delay(TimeSpan.FromSeconds(Interval));

                var activities = await api.GetActivitiesAsync(conversationId, watermark);
                if(activities != null)
                {
                    watermark = activities?.Watermark;
                    if(activities.Activities.Count > 0)
                        MessageReceived?.Invoke(activities);
                }
            }
        }

        #endregion

    }
}