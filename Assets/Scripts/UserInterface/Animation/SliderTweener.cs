using Aci.Unity.UI.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.Animation
{
    public class SliderTweener : Tweener<Slider, float>
    {
        protected override void ExecuteFrame(float percentage)
        {
            float t = m_Transition.Evaluate(percentage);
            m_Target.normalizedValue = Mathf.Lerp(fromValue, toValue, t);
        }
    }
}