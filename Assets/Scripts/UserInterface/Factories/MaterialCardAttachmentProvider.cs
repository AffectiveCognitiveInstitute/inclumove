using Aci.Unity.Bot;
using Aci.Unity.Models;
using Aci.Unity.UserInterface.ViewControllers;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.Factories
{
    /// <summary>
    /// Provider for <see cref="MaterialCard"/>
    /// </summary>
    public class MaterialCardAttachmentProvider : AttachmentProviderBase
    {
        private MonoMemoryPool<MaterialCardViewController> m_Pool;
        private List<AssemblyComponent> m_AssemblyComponents = new List<AssemblyComponent>();

        /// <inheritdoc/>
        public override string type => AttachmentContentType.Material;

        [Zenject.Inject]
        private void Construct(MonoMemoryPool<MaterialCardViewController> pool)
        {
            m_Pool = pool;
        }

        /// <inheritdoc/>
        public override GameObject GetGameObject(Activity activity)
        {
            var attachment = activity.Attachments.FirstOrDefault();
            var materialCard = attachment.GetRichCard<MaterialCard>();
            var vc = m_Pool.Spawn();

            if (materialCard.Materials != null)
            {
                m_AssemblyComponents.Clear();
                foreach(var m in materialCard.Materials)
                {
                    m_AssemblyComponents.Add(new AssemblyComponent(m.Image, m.Title, m.Meta, m.Amount));
                }
            }

            vc.Initialize(materialCard.Text, materialCard.Image?.Url, m_AssemblyComponents);
            return vc.gameObject;
        }
    }
}
