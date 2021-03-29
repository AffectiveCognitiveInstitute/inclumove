using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Scene;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class WorkflowViewController : MonoBehaviour,
                                          INavigatedAware,
                                          INavigatingAwayAware,
                                          INavigatedBackAware,
                                          IAciEventHandler<WorkflowStopArgs>
    {
        private WorkflowLoader m_WorkflowLoader;
        private IUserManager m_UserManager;
        private ITimeProvider m_TimeProvider;
        private IAciEventManager m_EventManager;
        private INavigationService m_NavigationService;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager
                             , WorkflowLoader workflowLoader
                             , IUserManager userManager
                             , ITimeProvider timeProvider
                             , INavigationService navigationService)
        {
            m_EventManager = eventManager;
            m_WorkflowLoader = workflowLoader;
            m_UserManager = userManager;
            m_TimeProvider = timeProvider;
            m_NavigationService = navigationService;
        }

        private void Start()
        {
            RegisterForEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            // Until we have a better way of selecting workflows, just load the first one available
            if(m_UserManager.CurrentUser.workflows != null)
            {
                string workflowFile = m_UserManager.CurrentUser.workflows.FirstOrDefault();
                if(!string.IsNullOrWhiteSpace(workflowFile))
                    m_WorkflowLoader.LoadWorkflow(workflowFile);
            }
        }

        public void OnNavigatingAway(INavigationParameters navigationParameters)
        {
            m_TimeProvider.paused = true;
        }

        public void OnNavigatedBack(INavigationParameters navigationParameters)
        {
            m_TimeProvider.paused = false;
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        public void OnEvent(WorkflowStopArgs arg)
        {
            m_NavigationService.PushAsync("Review_01", AnimationOptions.Synchronous, false);
        }
    }
}