using Aci.Unity.Events;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using UnityEngine;

namespace Aci.Unity.Gamification
{
    public class MilestoneAchievedEventListener : MonoBehaviour, IAciEventHandler<MilestoneAchievedArgs>
    {
        private IAciEventManager m_AciEventManager;
        private IMilestoneFacade m_MilestoneFacade;
        private IWorkflowService m_WorkflowService;

        [Zenject.Inject]
        private void Construct(IAciEventManager aciEventManager, IMilestoneFacade milestoneFacade, IWorkflowService workflowService)
        {
            m_AciEventManager = aciEventManager;
            m_MilestoneFacade = milestoneFacade;
            m_WorkflowService = workflowService;
        }

        private void OnEnable()
        {
            RegisterForEvents();
        }

        private void OnDisable()
        {
            UnregisterFromEvents();
        }

        public void OnEvent(MilestoneAchievedArgs arg)
        {
            // only show popup if not running workflow (else chat will be used)
            if(!m_WorkflowService.isRunning)
                m_MilestoneFacade.DisplayMilestoneAchieved(arg.milestone);
        }

        public void RegisterForEvents()
        {
            m_AciEventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_AciEventManager.RemoveHandler(this);
        }
    }
}