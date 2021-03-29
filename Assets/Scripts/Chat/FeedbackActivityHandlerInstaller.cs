using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Chat
{
    using BotConnector.Unity;

    public class FeedbackActivityHandlerInstaller : MonoInstaller<FeedbackActivityHandlerInstaller>
    {
        [SerializeField]
        private FeedbackActivityLibrary m_Library;

        [SerializeField]
        private Bot m_Bot; 

        public override void InstallBindings()
        {
            Container.Bind<FeedbackActivityLibrary>().FromInstance(m_Library);
            Container.BindFactory<PredefinedActivity, Activity, FeedbackActivityFactory>();
        }
    }
}
