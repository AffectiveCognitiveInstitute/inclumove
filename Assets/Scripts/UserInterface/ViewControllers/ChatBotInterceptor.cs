using Aci.Unity.Gamification;
using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using KobeluBot = BotConnector.Unity.Bot;
using Aci.Unity.Events;
using System.Linq;
using Aci.Unity.Audio;
using System;
using Aci.Unity.Bot;

namespace Aci.Unity.UserInterface
{

    public class ChatBotInterceptor : MonoBehaviour,
                                      IChatWindow,
                                      IAciEventHandler<UserLogoutArgs>
    {
        [SerializeField]
        private Transform m_ScrollViewContent;

        [SerializeField]
        private GameObject m_BotCardPrefab;

        [SerializeField]
        private GameObject m_UserCardPrefab;

        [SerializeField]
        private AudioClip m_NewMessageAudioClip;
        
        private Sprite m_UserSprite;

        private List<KeyValuePair<Activity, ChatCardViewController>> m_Messages = new List<KeyValuePair<Activity, ChatCardViewController>>(100);
        //private List<ChatCardViewController> m_Messages = new List<ChatCardViewController>(100);
        private int m_MessageBeginIndex = -1;
        private bool m_WasLastMessageFromBot;
        private IComparer<KeyValuePair<Activity, ChatCardViewController>> m_Comparer = new ActivityComparer();

        [Inject]
        private ChatCardViewController.Factory m_ChatCardFactory;

        [Inject]
        private KobeluBot m_Bot;
        
        [Inject]
        private IFactory<Activity, GameObject> m_ContentFactory;
        
        private IAciEventManager m_AciEventManager;
        private float m_LastAudioPlayTime;
        private IChatWindowFacade m_ChatWindowFacade;
        private IBotMessenger m_BotMessenger;
        private IUserManager m_UserManager;
        private IAudioService m_AudioService;
        private const float AudioIntervalTime = 1;

        private Activity m_LastActivity = null;

        [Zenject.Inject]
        private void Construct(IChatWindowFacade chatWindowFacade,
                               IBotMessenger botMessenger,
                               IAciEventManager eventManager,
                               IUserManager userManager,
                               IAudioService audioService)
        {
            m_ChatWindowFacade = chatWindowFacade;
            m_BotMessenger = botMessenger;
            m_AciEventManager = eventManager;
            m_UserManager = userManager;
            m_AudioService = audioService;
        }

        private void Awake()
        {
            m_ScrollViewContent = m_ScrollViewContent ?? GetComponent<Transform>();
        }

        private void OnEnable()
        {
            m_ChatWindowFacade.Register(this);
            m_Bot.MessageReceived.AddListener(OnBotMessageReceived);
            RegisterForEvents();
        }

        private void OnDisable()
        {
            m_ChatWindowFacade.Unregister();
            m_Bot.MessageReceived.RemoveListener(OnBotMessageReceived);
            UnregisterFromEvents();
        }

        private void OnBotMessageReceived(Activity activity)
        {
            if (activity.Type != "message")
                return;

            if (m_Messages.Any(x => x.Key.Id == activity.Id))
                return;

            if (m_LastActivity != null && 
                m_LastActivity.Value == activity.Value && 
                m_LastActivity.Text == activity.Text && 
                m_LastActivity.Name == activity.Name && 
                !(activity.Name == null && activity.Text == null))
            {
                m_BotMessenger?.ResendLastActivityAsync();
                return;
            }

            GameObject content = m_ContentFactory.Create(activity);

            if (content == null)
                return;


            ChatCardViewController card = CreateCard(m_BotCardPrefab);
            AddMessage(card.gameObject);

            // Set whether avatar should be enabled based on last message
            if (!m_WasLastMessageFromBot || m_MessageBeginIndex == -1)
            {
                card.SetAvatarEnabled(true);
                m_MessageBeginIndex = m_Messages.Count;
            }
            else
            {
                for (int i = m_MessageBeginIndex; i < m_Messages.Count; i++)
                    m_Messages[i].Value.SetAvatarEnabled(false);
            }

            m_LastActivity = activity;

            m_AciEventManager.Invoke(new BotMessageReceivedEvent());

            card.AddContent(content);

            m_WasLastMessageFromBot = true;
            //m_Messages.Add(card);
            m_Messages.Add(new KeyValuePair<Activity, ChatCardViewController>(activity, card));
            EnsureCorrectOrder();

            PlayNotificationSound();
        }

        private void PlayNotificationSound()
        {
            // This to prevent the sound being played multiple times when multiple messages
            // arrive simultaneously.
            if (Time.time - m_LastAudioPlayTime < AudioIntervalTime)
                return;

            // Play audio when a new message is received from bot
            try
            {
                m_AudioService.PlayAudioClip(m_NewMessageAudioClip, AudioChannels.Chat);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                m_LastAudioPlayTime = Time.time;
            }
        }

        private void EnsureCorrectOrder()
        {
            m_Messages.Sort(m_Comparer);
            for (int i = 0; i < m_Messages.Count; i++)
                m_Messages[i].Value.gameObject.transform.SetSiblingIndex(i);
        }

        public void OnUserMessageReceived()
        {
            ChatCardViewController card = CreateCard(m_UserCardPrefab);

            // Set whether avatar should be enabled based on last message
            if (m_WasLastMessageFromBot || m_MessageBeginIndex == -1)
            {
                card.SetAvatarEnabled(true);
                m_MessageBeginIndex = m_Messages.Count;
            }
            else
            {
                for (int i = m_MessageBeginIndex; i < m_Messages.Count; i++)
                    m_Messages[i].Value.SetAvatarEnabled(false);
            }

            //card.AddContent(m_ContentFactory.Create(activity));


            m_WasLastMessageFromBot = false;
            //m_Messages.Add(card);
            //m_Messages.Add(new KeyValuePair<Activity, ChatCardViewController>(activity, card));
            card.SetAvatarImage(m_UserManager.CurrentUser.userPicture);

        }

        private ChatCardViewController CreateCard(GameObject prefab)
        {
            return m_ChatCardFactory.Create(prefab);
        }

        public void RegisterForEvents()
        {
            m_AciEventManager.AddHandler<UserLogoutArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_AciEventManager.RemoveHandler<UserLogoutArgs>(this);
        }

        public async void OnEvent(UserLogoutArgs arg)
        {
            Clear();
            await m_Bot.EndConversation();
        }

        public void Clear()
        {
            m_Messages.ForEach(kvp => Destroy(kvp.Value.gameObject));
            m_Messages.Clear();
        }

        public void AddMessage(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            gameObject.transform.SetParent(m_ScrollViewContent, false);
            gameObject.transform.SetAsLastSibling();
        }

        internal class ActivityComparer : IComparer<KeyValuePair<Activity, ChatCardViewController>>
        {
            public int Compare(KeyValuePair<Activity, ChatCardViewController> x, KeyValuePair<Activity, ChatCardViewController> y)
            {
                return x.Key.Timestamp.Value.CompareTo(y.Key.Timestamp.Value);
            }
        }
    }
}