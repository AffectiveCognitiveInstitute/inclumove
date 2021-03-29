// <copyright file=IAvatarMemory.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
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
// <date>02/27/2019 13:46</date>

using System;
using System.Threading.Tasks;
using Aci.Unity.Events;
using Aci.Unity.Models;
using Aci.Unity.Network;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Zenject;

namespace Aci.Unity.Gamification
{
    public class AvatarMemory : MonoBehaviour
                              , IAciEventHandler<EmotionChanged>
                              , IAciEventHandler<WorkflowStepEndedArgs>
                              , IAciEventHandler<UserLoginArgs>

    {
        private IAciEventManager m_Broker;

        [Flags]
        public enum Knowledge
        {
            None = 0,
            JustLoggedIn = 1 << 0,
            JustChangedEmotion = JustLoggedIn << 1,
            JustAdvancedStep = JustChangedEmotion << 1,
            JustHadIncorrectStep = JustAdvancedStep << 1
        }

        [Range(1, 10), Tooltip("Timespan for which the Avatar will retain it's memory (in seconds).")]
        public float retentionTime;

        private Knowledge m_CurKnowledge;
        public Knowledge currentKnowledge => m_CurKnowledge;

        [Inject]
        public void Construct(IAciEventManager manager)
        {
            m_Broker = manager;
            RegisterForEvents();
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            m_Broker?.AddHandler<EmotionChanged>(this);
            m_Broker?.AddHandler<WorkflowStepEndedArgs>(this);
            m_Broker?.AddHandler<UserLoginArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            m_Broker?.RemoveHandler<EmotionChanged>(this);
            m_Broker?.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_Broker?.RemoveHandler<UserLoginArgs>(this);
        }

        /// <inheritdoc />
        public async void OnEvent(UserLoginArgs arg)
        {
            m_CurKnowledge |= Knowledge.JustLoggedIn;
            await Task.Delay((int)(retentionTime * 1000));
            m_CurKnowledge ^= Knowledge.JustLoggedIn;
        }

        /// <inheritdoc />
        public async void OnEvent(WorkflowStepEndedArgs arg)
        {
            m_CurKnowledge |= Knowledge.JustAdvancedStep;
            await Task.Delay((int)(retentionTime * 1000));
            m_CurKnowledge ^= Knowledge.JustAdvancedStep;
        }

        /// <inheritdoc />
        public async void OnEvent(EmotionChanged arg)
        {
            m_CurKnowledge |= Knowledge.JustChangedEmotion;
            await Task.Delay((int)(retentionTime*1000));
            m_CurKnowledge ^= Knowledge.JustChangedEmotion;
        }
    }
}