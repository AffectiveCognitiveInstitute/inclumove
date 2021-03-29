using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aci.Unity.Scene.SceneItems
{
    public interface IPayloadViewController
    {
        PayloadType payloadType { get; }
        string payload { get; }
        float delay { get; set; }
        void SetPayload(PayloadType type, string payload, float delay);

    }
}
