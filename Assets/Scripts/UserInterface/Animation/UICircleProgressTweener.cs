using Aci.Unity.UI.Tweening;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface.Animation
{
    public class UICircleProgressTweener : Tweener<UICircle, float>
    {
        protected override void ExecuteFrame(float percentage)
        {
            float t = m_Transition.Evaluate(percentage);
            m_Target.SetProgress(Mathf.Lerp(m_FromValue, m_ToValue, t));
        }
    }
}