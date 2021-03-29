using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityScript.Steps;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public interface ISubHandle
    {
        SelectionHandle.HandleActiveEvent handleActive { get; }

        void ToggleActive(bool enabled, bool silent);

        void ToggleHidden(bool hidden);
    }
}
