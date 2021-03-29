using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Threading.Tasks;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace BotConnector.Unity
{
    public enum ConnectionStatus
    {
        None,
        Connecting,
        Connected,
        ConnectionFailed,
        TokenExpired,
        Ended
    }

    public enum Connectivity
    {
        Websocket,
        PollingOverHTTP
    }

    [Serializable]
    public class ConnectionStatusEvent : UnityEvent<ConnectionStatus> { }

    public class BotException : Exception
    {
        public BotException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Client for connecting to a bot using the Direct Line v3.0 API.
    /// </summary>
    [HelpURL("https://docs.microsoft.com/de-de/bot-framework/rest-api/bot-framework-rest-direct-line-3-0-api-reference")]
    public class Bot : MonoBehaviour, IBot
    {
        #region Consts

        public const string DirectLineEndpoint = "https://directline.botframework.com";

        private const int TokenLifeTime = 1800;         // 30 minutes in seconds
        private const int TokenRefreshInterval = 900;   // 15 minutes in seconds
        private const int TokenRefreshRetries = 3;

        private const string ConversationIdKey = "conversationId";
        private const string WatermarkKey = "watermark";

        #endregion

        #region Private Fields

        private IConnectionStrategy connection;
        private DirectLineClient client;
        private ChannelAccount account;

        private string watermark;

        [SerializeField]
        private string secret;

        [SerializeField]
        private Connectivity connectivity = Connectivity.Websocket;

        [SerializeField]
        private int pollingInterval = 1;

        [SerializeField]
        private bool autoStart = true;

        [SerializeField]
        private bool onlyBotMessages = true;

        [SerializeField]
        private string domain;

        [SerializeField]
        private string userId = "Default-User", userName = "Default-User";

        [SerializeField]
        private string handle;

        [SerializeField]
        private ConnectionStatusEvent onConnectionStatusChanged = new ConnectionStatusEvent();

        [SerializeField]
        private ActivityEvent onMessageReceived = new ActivityEvent();

        [SerializeField]
        private ActivityEvent onSystemActivityReceived = new ActivityEvent();

        [SerializeField]
        private float messageDelay = 3;

        private DateTime m_LastTime = DateTime.Now;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to start the conversation as soon as possible.
        /// </summary>
        public bool AutoStart
        {
            get { return autoStart; }
            set { autoStart = value; }
        }

        /// <summary>
        /// Gets or sets whether to propagate only messages from the bot.
        /// </summary>
        public bool OnlyBotMessages
        {
            get { return onlyBotMessages; }
            set { onlyBotMessages = value; }
        }

        /// <summary>
        /// Gets or sets the domain for requests.
        /// </summary>
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <inheritdoc/>
        public string Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        /// <inheritdoc/>
        public ConnectionStatusEvent ConnectionStatusChanged
        {
            get { return onConnectionStatusChanged; }
        }

        /// <inheritdoc/>
        public ActivityEvent MessageReceived
        {
            get { return onMessageReceived; }
        }

        /// <inheritdoc/>
        public ActivityEvent SystemActivityReceived
        {
            get { return onSystemActivityReceived; }
        }

        /// <inheritdoc/>
        public ChannelAccount Account
        {
            get
            {
                if (account.Id != userId || account.Name != userName)
                    account = new ChannelAccount(userId, userName);
                return account;
            }
            set
            {
                account = value;
                userId = account.Id;
                userName = account.Name;
            }
        }

        /// <inheritdoc/>
        public string ConversationId { get; private set; }

        /// <inheritdoc/>
        public ConnectionStatus Status { get; private set; } = ConnectionStatus.None;

        /// <inheritdoc/>
        public IBotStorage Storage { get; set; } = new EphemeralStorage();

        /// <inheritdoc/>
        public IConnectionStrategy Connection
        {
            get { return connection; }
            set
            {
                if(connection != null)
                    connection.MessageReceived -= PublishActivities;

                connection = value;
                connection.MessageReceived += PublishActivities;
            }
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            account = new ChannelAccount();
            client = new DirectLineClient(secretOrToken: secret);
            client.BaseUri = new Uri(Domain);

            if (AutoStart)
                Task.Run(() => StartConversation()).ConfigureAwait(false);
        }

        private void OnDisable()
        {
            Connection?.DisconnectAsync();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public async Task StartConversation()
        {
            if (ConversationId != null)
                await EndConversation();

            ChangeStatus(ConnectionStatus.Connecting);

            if(Connection == null)
                InitializeConnectionStrategy();

            try
            {
                var conversation = await client.Conversations.StartConversationAsync();
                await Connection.ConnectAsync(conversation).ConfigureAwait(false);

                ConversationId = conversation.ConversationId;
                ChangeStatus(ConnectionStatus.Connected);

#pragma warning disable
                StartRefreshTokenLoop().ConfigureAwait(false);
#pragma warning restore
            }
            catch (Exception e)
            {
                ChangeStatus(ConnectionStatus.None);

                Debug.LogException(e);
                Debug.LogError($"[BOT] Unable to create a conversation.");
            }
        }

        /// <inheritdoc/>
        public async Task EndConversation()
        {
            if (Status == ConnectionStatus.Connected)
            {
                var activity = new Activity(type: ActivityTypes.EndOfConversation);
                await SendActivityAsync(activity).ConfigureAwait(false);
                await Connection.DisconnectAsync().ConfigureAwait(false);
            }

            ConversationId = null;
            Status = ConnectionStatus.Ended;
        }

        /// <inheritdoc/>
        public async Task SendActivityAsync(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            if (Status != ConnectionStatus.Connected)
                throw new Exception("The bot is not yet ready. Try again later.");

            activity.From = Account;

            await client.Conversations.PostActivityAsync(ConversationId, activity).ConfigureAwait(false);
        }

        private async Task StartRefreshTokenLoop()
        {
            while (Status == ConnectionStatus.Connected)
            {
                await Task.Delay(TimeSpan.FromSeconds(TokenRefreshInterval));
                var isSuccess = await RetryToRefreshToken();

                if(isSuccess)
                {
                    // ??
                }
                else
                {
                    ChangeStatus(ConnectionStatus.TokenExpired);
                }
            }
        }

        private async Task<bool> RetryToRefreshToken()
        {
            int retries = 1;

            while (retries <= TokenRefreshRetries)
            {
                Debug.Log("[BOT] Requesting new token");
                var conversation = await client.Tokens.RefreshTokenAsync();

                if (conversation?.Token != null)
                {
                    Debug.Log("[BOT] Successfully requested token");
                    return true;
                }

                Debug.Log("[BOT] Error requesting token. Trying again in 5 minutes...");
                await Task.Delay(TimeSpan.FromMinutes(5));

                retries++;
            }

            return false;
        }

        private void ChangeStatus(ConnectionStatus status)
        {
            if (Status == status)
                return;

            Status = status;
            ConnectionStatusChanged.Invoke(status);
        }

        private async void PublishActivities(ActivitySet set)
        {
            //Debug.Log($"[BOT] Received activity set [{set.Activities.Count}]");
            foreach (var activity in set.Activities)
            {
                if (activity.Type == "message")
                {
                    if (OnlyBotMessages && activity.From?.Id == Account.Id)
                    {
                        continue;
                    }

                    UnityMainThreadDispatcher.Instance().Enqueue(() => MessageReceived.Invoke(activity));

                    float messageDelta = (float)DateTime.Now.Subtract(m_LastTime).TotalSeconds;
                    m_LastTime = DateTime.Now;
                    if (messageDelta < messageDelay)
                        await Task.Delay(TimeSpan.FromSeconds(messageDelay - messageDelta));
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => SystemActivityReceived.Invoke(activity));
                }
            }
        }

        private void InitializeConnectionStrategy()
        {
            switch (connectivity)
            {
                case Connectivity.Websocket:
                    Connection = new WebsocketConnectionStrategy();
                    break;
                case Connectivity.PollingOverHTTP:
                    Connection = new PollingConnectionStrategy(client.Conversations, pollingInterval);
                    break;
                default:
                    break;
            }
        }

        public void SimulateMessageReceived(Activity activity)
        {
            PublishActivities(new ActivitySet(new List<Activity>()
            {
                activity
            }));
        }

        #endregion

    }
}


