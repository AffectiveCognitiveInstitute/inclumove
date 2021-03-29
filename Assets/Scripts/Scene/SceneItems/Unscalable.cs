using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.Scene.SceneItems
{
    public class Unscalable : MonoBehaviour, IScalable
    {
        public Vector3 size
        {
            get => Vector3.one;
            set
            {
                //do nothing
            }
        }
    }
}
