// <copyright file=IdDisposalService.cs/>
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
// <date>10/31/2019 18:41</date>

using System;
using System.Collections.Generic;

namespace Aci.Unity.Util
{
    public class IdDisposalService : IIdDisposalService
    {
        private Dictionary<string, List<IDisposable>> m_GuidDisposables = new Dictionary<string, List<IDisposable>>();
        private Dictionary<int, List<IDisposable>> m_IntDisposables = new Dictionary<int, List<IDisposable>>();
        private Dictionary<uint, List<IDisposable>> m_UIntDisposables = new Dictionary<uint, List<IDisposable>>();

        public void Register(IDisposable disposable, string id)
        {
            if(!m_GuidDisposables.ContainsKey(id))
                m_GuidDisposables.Add(id, new List<IDisposable>());
            if (m_GuidDisposables[id].Contains(disposable))
                return;
            m_GuidDisposables[id].Add(disposable);
        }

        public void Register(IDisposable disposable, int id)
        {
            if (!m_IntDisposables.ContainsKey(id))
                m_IntDisposables.Add(id, new List<IDisposable>());
            if (m_IntDisposables[id].Contains(disposable))
                return;
            m_IntDisposables[id].Add(disposable);
        }

        public void Register(IDisposable disposable, uint id)
        {
            if (!m_UIntDisposables.ContainsKey(id))
                m_UIntDisposables.Add(id, new List<IDisposable>());
            if (m_UIntDisposables[id].Contains(disposable))
                return;
            m_UIntDisposables[id].Add(disposable);
        }

        public void Dispose(string id)
        {
            if (!m_GuidDisposables.ContainsKey(id))
                return;
            foreach (IDisposable disposable in m_GuidDisposables[id])
            {
                disposable.Dispose();
            }
            m_GuidDisposables[id].Clear();
            m_GuidDisposables.Remove(id);
        }

        public void Dispose(int id)
        {
            if (!m_IntDisposables.ContainsKey(id))
                return;
            foreach (IDisposable disposable in m_IntDisposables[id])
            {
                disposable.Dispose();
            }
            m_IntDisposables[id].Clear();
            m_IntDisposables.Remove(id);
        }

        public void Dispose(uint id)
        {
            if (!m_UIntDisposables.ContainsKey(id))
                return;
            foreach (IDisposable disposable in m_UIntDisposables[id])
            {
                disposable.Dispose();
            }
            m_UIntDisposables[id].Clear();
            m_UIntDisposables.Remove(id);
        }
    }
}