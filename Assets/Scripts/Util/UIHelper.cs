// <copyright file=UIHelper.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

using System.Collections;
using UnityEngine;

namespace Aci.Unity.Util
{
    public static class AnimationHelper
    {
        /// <summary>
        ///     Disables the target gameobject holding animator after taget animation has finished
        /// </summary>
        /// <param name="anim">Animator</param>
        /// <param name="animationName">target animation name</param>
        /// <param name="animationParameter">animation parameter as bool</param>
        /// <param name="animationEndState">target animation end state as string</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator DisableAfterAnimation(Animator anim, string animationName, bool animationParameter,
                                                        string   animationEndState)
        {
            anim.SetBool(Animator.StringToHash(animationName), animationParameter);
            bool closedStateReached = false;
            bool wantToClose = true;
            while (!closedStateReached && wantToClose)
            {
                if (!anim.IsInTransition(0))
                    closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(animationEndState);

                wantToClose = animationParameter != anim.GetBool(Animator.StringToHash(animationName));

                yield return new WaitForEndOfFrame();
            }

            if (wantToClose)
                anim.gameObject.SetActive(false);
        }
    }
}