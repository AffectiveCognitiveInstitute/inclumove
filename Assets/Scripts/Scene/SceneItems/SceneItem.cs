using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.UI.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene.SceneItems
{
    public class SceneItem : MonoBehaviour, ISceneItem
    {
        public class Factory : PlaceholderFactory<SceneItemData, ISceneItem> { }

        /// <inheritdoc />
        public Transform itemTransform => transform;

        private IIdentifiable<int> m_Identifiable;
        /// <inheritdoc />
        public IIdentifiable<int> identifiable => m_Identifiable;

        private IPayloadViewController m_PayloadViewController;
        /// <inheritdoc />
        public IPayloadViewController payloadViewController => m_PayloadViewController;

        private IColorable m_Colorable;
        /// <inheritdoc />
        public IColorable colorable => m_Colorable;

        private ILevelable m_Levelable;
        /// <inheritdoc />
        public ILevelable levelable => m_Levelable;

        private IScalable m_Scalable;
        /// <inheritdoc />
        public IScalable scalable => m_Scalable;

        [Zenject.Inject]
        public void Construct(IIdentifiable<int> identifiable, IPayloadViewController payloadViewController,
                              IColorable         colorable,    ILevelable             levelable, IScalable scalable)
        {
            m_Identifiable = identifiable;
            m_PayloadViewController = payloadViewController;
            m_Colorable = colorable;
            m_Levelable = levelable;
            m_Scalable = scalable;
        }
    }
}
