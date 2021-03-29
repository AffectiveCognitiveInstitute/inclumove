using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.Factories
{
    /// <summary>
    /// Provider for cards with text content
    /// </summary>
    public class TextCardAttachmentProvider : AttachmentProviderBase
    {
        private MonoMemoryPool<TextCardViewController> m_Pool;

        /// <inheritdoc/>
        public override string type => AttachmentContentType.Default;

        [Inject]
        private void Construct(MonoMemoryPool<TextCardViewController> pool)
        {
            m_Pool = pool;
        }

        /// <inheritdoc/>
        public override GameObject GetGameObject(Activity activity)
        {
            TextCardViewController vc = m_Pool.Spawn();
            vc.Initialize(activity.Text);

            return vc.gameObject;
        }
    }
}
