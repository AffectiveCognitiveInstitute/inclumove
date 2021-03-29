using Aci.UI.Binding;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Quests
{
    public class QuestContentViewController : MonoBindable
    {
        public class Factory : PlaceholderFactory<RectTransform, QuestContent, QuestContentViewController>
        {
            private readonly DiContainer m_DiContainer;

            public override QuestContentViewController Create(RectTransform param1, QuestContent param2)
            {
                QuestContentViewController vc = CreateInternal(new List<TypeValuePair>
                {
                    new TypeValuePair{Type = param2.GetType(), Value = param2}
                });
                if (param1 != null)
                    vc.transform.SetParent(param1, false);
                return vc;
            }
        }
    }
}