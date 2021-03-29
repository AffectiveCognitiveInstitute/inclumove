using Aci.UI.Binding;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Dialog;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Workflow;

namespace Aci.Unity.UserInterface.ViewControllers
{

    public class PauseViewController : MonoBindable
    {
        private IDialogFacade m_DialogFacade;
        private INavigationService m_NavigationService;
        private IWorkflowService m_WorkflowService;

        [Zenject.Inject]
        private void Construct(IDialogFacade dialogFacade,
                               INavigationService navigationService,
                               IWorkflowService workflowService)
        {
            m_DialogFacade = dialogFacade;
            m_NavigationService = navigationService;
            m_WorkflowService = workflowService;
        }

        public void OnQuitApplicationButtonClicked()
        {
            m_DialogFacade.DisplayAlert("Programm Beenden", "Möchtest Du für heute wirklich aufhören?", "Nein", "Ja", () =>
            {
                m_WorkflowService.StopWork();
                m_NavigationService.PushWithNewStackAsync("Review_01", AnimationOptions.Asynchronous);       
            });
        }
    }
}

