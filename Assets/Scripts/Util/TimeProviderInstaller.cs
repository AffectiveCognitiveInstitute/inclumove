// <copyright file=ModuleInstaller.cs/>
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
// <date>07/24/2018 10:12</date>

using System;
using Zenject;
using UnityEngine;
#if UNITY_EDITOR
using NSubstitute;
#endif

namespace Aci.Unity.Util
{
    public class TimeProviderInstaller : MonoInstaller<TimeProviderInstaller>
    {
        [SerializeField, Tooltip("Editor Only: Uses a mocked version of TimeProvider.")]
        private bool m_UseMock = false;

        public override void InstallBindings()
        {
#if UNITY_EDITOR
            if(!m_UseMock)
                Container.Bind<ITimeProvider>().To<AsyncTimeProvider>().AsSingle();
            else
                Container.Bind<ITimeProvider>().FromSubContainerResolve().ByInstaller<MockTimeProviderInstaller>().AsSingle();
#else
            Container.Bind<ITimeProvider>().To<AsyncTimeProvider>().AsSingle();
#endif
        }

#if UNITY_EDITOR
        public class MockTimeProviderInstaller : Installer<MockTimeProviderInstaller>
        {
            public override void InstallBindings()
            {
                ITimeProvider timeProvider = Substitute.For<ITimeProvider>();
                timeProvider.elapsedTotal.Returns(TimeSpan.FromSeconds(UnityEngine.Random.Range(60f * 60f, 60f * 60f * 5)));
                Container.Bind<ITimeProvider>().FromInstance(timeProvider);
            }
        }
#endif
    }
}