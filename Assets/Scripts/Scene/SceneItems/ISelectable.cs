using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Aci.Unity.Scene.SceneItems
{
    public interface ISelectable
    {
        UnityEvent<ISelectable> selectionChanged { get; }

        bool isSelected { get; set; }
    }
}
