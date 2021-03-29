using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Handles in-and outgoing activites for a bot.
    /// </summary>
    [ExecuteInEditMode]
    public class Chat : MonoBehaviour, IChat
    {
        private const string HistoryFileKey = "ChatHistoryFilePath";

        #region Private fields

        private ScrollRectTweener scroller;
        private IBot conversation;
        private IBot bot;
        private List<Activity> history = new List<Activity>(); 

        [SerializeField]
        private bool saveAndRestoreChat;

        [SerializeField]
        private GameObject[] inputs;

        [SerializeField]
        private GameObject chatPanel;

        [SerializeField]
        private GameObject suggestedActionsContainer;

        [SerializeField]
        private GameObject suggestedActionsContent;

        #endregion

        #region Properties

        public bool SaveAndRestoreChat
        {
            get { return saveAndRestoreChat; }
            set { saveAndRestoreChat = value; }
        }


        public GameObject ChatPanel
        {
            get { return chatPanel; }
            set { chatPanel = value; }
        }

        public IBot Bot
        {
            get { return bot; }
            set
            {
                if (bot == value)
                    return;

                SetupBot(value);
            }
        }

        /// <inheritdoc/>
        public IRenderSettings Settings { get; set; }

        #endregion

        #region Unity methods

        private void Start()
        {
            scroller = ChatPanel?.GetComponentInParent<ScrollRectTweener>();

            if (Bot == null)
                Bot = GetComponent<IBot>();

            if (Settings == null)
                Settings = GetComponent<IRenderSettings>();

            Bot.Storage.Load()
                .Then(store => GetHistoryInfoFromStore(store));

            SetupInputs();
        }

        private void OnDisable()
        {
            if (Bot == null || !Bot.Storage.Store.ContainsKey(HistoryFileKey))
                return;

            var file = Bot.Storage.Store[HistoryFileKey];
            var serializer = new JsonSerializer();

            using (var streamWriter = File.CreateText(file))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonWriter, history);
            }
        }

        #endregion

        /// <inheritdoc/>
        public IPromise<ResourceResponse> SendToBot(Activity activity)
        {
            activity.Type = "message";

            var response =  bot.SendActivityAsync(activity);
            throw new NotImplementedException();
            //var context = Settings.CreateContextForUser(new Promise((res, rej) => { response.Then(_ => res(), rej); }));

            //var render = context.Render(activity);
            //render.transform.SetParent(ChatPanel.transform);
            //ScrollToBottom();

            //history.Add(activity);

            //return null;
        }

        private IPromise<ResourceResponse> SendToBot(string message)
            => SendToBot(new Activity(text: message));

        private void SetupInputs()
        {
            if (inputs == null)
                return;

            foreach (var inputGameObject in inputs)
            {
                if (inputGameObject != null)
                {
                    var chatInput = inputGameObject.GetComponent<INotifyChatInput>();
                    if (chatInput != null)
                        chatInput.ChatInputReceived += input => SendToBot(input);
                }
            }
        }

        private void OnMessageReceived(Activity activity)
        {
            history.Add(activity);

            var context = Settings.CreateContextForBot();
            UnityMainThreadDispatcher.Instance().Enqueue(() => Render(activity, context));
        }

        private void Render(Activity activity, IRenderContext context)
        {
            RenderActivity(activity, context);
            RenderSuggestedActions(activity, context);

            ScrollToBottom();
        }

        private void RenderActivity(Activity activity, IRenderContext context)
        {
            var render = context.Render(activity);
            render?.transform.SetParent(ChatPanel.transform);
        }

        private void RenderSuggestedActions(Activity activity, IRenderContext context)
        {
            if (activity.SuggestedActions != null &&
                activity.SuggestedActions.Actions != null)
            {
                suggestedActionsContainer?.SetActive(true);
                RenderSuggestedActionItems(context, activity.SuggestedActions);
            }
            else
            {
                ClearSuggestedActions();
                suggestedActionsContainer?.SetActive(false);
            }
        }

        private void RenderSuggestedActionItems(IRenderContext context, SuggestedActions suggestedActions)
        {
            if (suggestedActionsContent == null)
                return;

            foreach (var suggestedAction in suggestedActions.Actions)
            {
                var template = Instantiate(context.Theme.Button);
                template.transform.SetParent(suggestedActionsContent.transform);

                var button = template.GetComponent<Button>();
                context.RenderToTarget(button.gameObject, suggestedAction.Title);
                button.onClick.AddListener(() =>
                {
                    context.InvokeCardAction(suggestedAction);
                    ClearSuggestedActions();
                });
            }
        }

        private void ClearSuggestedActions()
        {
            foreach (Transform child in suggestedActionsContent.transform)
                Destroy(child.gameObject);

            suggestedActionsContainer.SetActive(false);
        }

        private void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }

        private IEnumerator ScrollToBottomCoroutine()
        {
            if (scroller != null)
            {
                yield return null;
                scroller.ScrollVertical(0);
            }
        }

        private void SetupBot(IBot value)
        {
            bot?.MessageReceived.RemoveListener(OnMessageReceived);
            bot?.ConnectionStatusChanged.RemoveListener(OnStatusChanged);
            bot = value;
            bot.MessageReceived.AddListener(OnMessageReceived);
            bot.ConnectionStatusChanged.AddListener(OnStatusChanged);
        }

        private void OnStatusChanged(ConnectionStatus status)
        {
            if (status == ConnectionStatus.Connected)
            {
            }
        }

        private void GetHistoryInfoFromStore(IStore store)
        {
            if (!saveAndRestoreChat)
                return;

            if (store.ContainsKey(HistoryFileKey))
            {
                var path = store[HistoryFileKey];
                LoadHistoryFromFile(path);
            }
            else
            {
                store.AddOrReplace(HistoryFileKey, Path.Combine(Application.persistentDataPath, "chatHistory.txt"));
            }
        }

        private void LoadHistoryFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(filePath))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var activities = serializer.Deserialize<List<Activity>>(jsonTextReader);

                var botContext = Settings.CreateContextForBot();
                var userContext = Settings.CreateContextForUser(Promise.Resolved());

                activities.ForEach(a =>
                {
                    Render(a, a.From.Id == Bot.Handle ? botContext : userContext);
                });
            }
        }
    }

}
