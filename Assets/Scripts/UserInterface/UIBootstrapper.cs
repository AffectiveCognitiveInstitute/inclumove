using Aci.Unity.Logging;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Util;
using System;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class UIBootstrapper : MonoBehaviour
    {
        [ConfigValue("mode")]
        public string mode { get; set; }

        private INavigationService m_NavigationService;
        private IConfigProvider m_ConfigProvider;

        [Zenject.Inject]
        private void Construct(INavigationService navigationService, IConfigProvider configProvider)
        {
            m_NavigationService = navigationService;
            m_ConfigProvider = configProvider;
        }

        private void Awake()
        {
            m_ConfigProvider?.RegisterClient(this);
        }

        private void Start()
        {
            AciLog.Log(nameof(UIBootstrapper), $"Starting UI in {mode} mode.");

            NavigationParameters navigationParameters = new NavigationParameters();
            switch(mode)
            {
                case "nochat":
                    throw new NotSupportedException("No Chat is no longer supported!");
                case "guest":
                    navigationParameters.Add("isGuest", true);
                    m_NavigationService.PushAsync("Idle", navigationParameters, AnimationOptions.Synchronous, false);
                    break;
                case "kiosk":
                default:
                    navigationParameters.Add("isGuest", false);
                    m_NavigationService.PushAsync("Idle", navigationParameters, AnimationOptions.Synchronous, false);
                    break;
            }
        }

        private void OnDestroy()
        {
            m_ConfigProvider?.UnregisterClient(this);
        }
    }
}

