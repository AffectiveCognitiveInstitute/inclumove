using System.Collections;
using UnityEngine;

namespace Aci.Unity.Extensions
{
    public static class AnimatorExtensions
    {
        public static IEnumerator SetTriggerAndWaitForCompletion(this Animator animator, string trigger, int layer = 0)
        {
            return SetTriggerAndWaitForCompletion(animator, Animator.StringToHash(trigger));
        }

        public static IEnumerator SetTriggerAndWaitForCompletion(this Animator animator, int triggerHash, int layer = 0)
        {
            animator.SetTrigger(triggerHash);
            return WaitForCurrentState(animator, layer);
        }

        public static IEnumerator WaitForCurrentState(this Animator animator, int layer = 0)
        {
            // Wait one frame in case a transition has previously been triggered.
            yield return null;

            if(animator.IsInTransition(layer))
            {
                // Wait for transition to complete
                while (animator.IsInTransition(layer))
                    yield return null;
            }

            while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
            {
                //Wait every frame until animation has finished
                yield return null;
            }
        }
    }
}

