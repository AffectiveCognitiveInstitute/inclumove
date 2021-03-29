using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Util;
using System;
using Zenject;

namespace Aci.Unity.Adaptivity
{
    public class AdaptivityService : IAdaptivityService
                                   , IInitializable
                                   , IDisposable
                                   , IAciEventHandler<UserLoginArgs>
                                   , IAciEventHandler<UserLogoutArgs>
    {
        private static readonly byte MaxLevel = 2;
        private static readonly byte MinLevel = 0;

        private IAciEventManager m_EventManager;
        private IUserManager m_UserManager;
        private IUserProfile m_User;
        private byte m_Level;

        /// <inheritdoc />
        public event AdaptivityLevelChangedDelegate adaptivityLevelChanged;

        /// <inheritdoc />
        public byte level
        {
            get => m_Level;
            set
            {
                if (value < MinLevel || value > MaxLevel)
                    return;

                if (m_Level == value)
                    return;

                byte previous = m_Level;
                m_Level = value;
                if(m_User != null)
                {
                    m_User.adaptivityLevel = value;
                }
                adaptivityLevelChanged?.Invoke(new AdaptivityLevelChangedEventArgs()
                {
                    newLevel = value,
                    previousLevel = previous
                });
            }
        }

        /// <inheritdoc />
        public byte maxLevel => MaxLevel;

        /// <inheritdoc />
        public byte minLevel => MinLevel;

        public AdaptivityService(IAciEventManager eventManager, IUserManager userManager)
        {
            m_EventManager = eventManager;
            m_UserManager = userManager;
            m_User = userManager.CurrentUser;
            level = m_User?.adaptivityLevel ?? 2;
        }

        public void Initialize()
        {
            RegisterForEvents();
        }

        public void Dispose()
        {
            UnregisterFromEvents();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<UserLoginArgs>(this);
            m_EventManager.AddHandler<UserLogoutArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<UserLoginArgs>(this);
            m_EventManager.RemoveHandler<UserLogoutArgs>(this);
        }

        public void OnEvent(UserLoginArgs arg)
        {
            m_User = m_UserManager.CurrentUser;
            level = m_User?.adaptivityLevel ?? 2;
        }

        public void OnEvent(UserLogoutArgs arg)
        {
            m_User = null;
            level = 2;
        }
    }
}