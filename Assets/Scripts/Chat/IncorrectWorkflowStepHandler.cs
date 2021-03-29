using Aci.Unity.Logging;
using BotConnector.Unity;
using UnityEngine;

namespace Aci.Unity.Chat
{
    public class IncorrectWorkflowStepHandler : MonoBehaviour
    {
        private IBot m_Bot;
        private IncorrectWorkflowStepActivityFactory m_Factory;

        [Zenject.Inject]
        private void Construct(IBot bot,
                               IncorrectWorkflowStepActivityFactory factory)
        {
            m_Bot = bot;
            m_Factory = factory;
        }
    }
}
