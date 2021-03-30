using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.Data;

namespace Aci.Unity.Util
{
    public interface IIdDisposalService
    {
        void Register(IDisposable disposable, string id);
        void Register(IDisposable disposable, int id);
        void Register(IDisposable disposable, uint id);
        void Dispose(string id);
        void Dispose(int id);
        void Dispose(uint id);
    }
}
