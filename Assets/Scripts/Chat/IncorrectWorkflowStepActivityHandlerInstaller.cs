using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Chat
{
    using BotConnector.Unity;

    public class IncorrectWorkflowStepActivityHandlerInstaller : MonoInstaller<IncorrectWorkflowStepActivityHandlerInstaller>
    {
        [SerializeField]
        private IncorrectWorkflowStepActivityLibrary m_Library;

        [SerializeField]
        private Bot m_Bot; 

        public override void InstallBindings()
        {
            Container.Bind<string>().FromInstance(m_Bot.Domain);
            Container.Bind<IncorrectWorkflowStepActivityLibrary>().FromInstance(m_Library);
            Container.BindFactory<Activity, IncorrectWorkflowStepActivityFactory>();
        }
    }
}
