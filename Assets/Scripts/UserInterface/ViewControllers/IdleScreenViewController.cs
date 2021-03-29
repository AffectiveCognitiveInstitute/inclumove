using Aci.Unity.Events;
using Aci.Unity.UI.Navigation;
using UnityEngine;
using Aci.Unity.Gamification;
using WebSocketSharp;
using Aci.Unity.Util;
using System;
using System.Text;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class IdleScreenViewController : MonoBehaviour, IAciEventHandler<USBDevice>
    {
        [ConfigValue("SkipIntro")]
        public bool skipIntro { get; set; }

        private IUserManager m_UserManager;
        private IAciEventManager m_EventManager;
        private INavigationService m_NavigationService;
        private IConfigProvider m_ConfigProvider;
        private IMilestoneFacade m_MilestoneFacade;
        private UsbDetectorService m_USBService;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager,
                               INavigationService navigationService,
                               IUserManager userManager,
                               IConfigProvider configProvider,
                               IMilestoneFacade milestoneFacade,
                               UsbDetectorService usbService)
        {
            m_UserManager = userManager;
            m_EventManager = eventManager;
            m_NavigationService = navigationService;
            m_ConfigProvider = configProvider;
            m_MilestoneFacade = milestoneFacade;
            m_USBService = usbService;
        }

        private void OnEnable()
        {
            m_ConfigProvider.RegisterClient(this);
            RegisterForEvents();
            m_USBService.active = true;
        }

        private void OnDisable()
        {
            m_ConfigProvider.UnregisterClient(this);
            UnregisterFromEvents();
        }

        private void OnDestroy()
        {
            m_ConfigProvider.UnregisterClient(this);
            UnregisterFromEvents();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        async void IAciEventHandler<USBDevice>.OnEvent(USBDevice arg)
        {
            if (arg.drive == null)
                return;

            bool userLoaded = await m_UserManager.LoadUser();

            m_USBService.active = false;

            if (userLoaded)
            {
                await m_NavigationService.PushAsync(skipIntro ? "Workflow" : "Welcome01", AnimationOptions.Synchronous, false);
                m_MilestoneFacade.StartMilestoneQuests();
            }
            else
                await m_NavigationService.PushAsync("Registration", AnimationOptions.Synchronous, false);
        }
    }
}