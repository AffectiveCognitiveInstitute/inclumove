using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Util
{
    public class ActivityLogger : MonoBehaviour
    {
        [SerializeField]
        private string m_Category;
        [SerializeField]
        private string m_Content;

        // Start is called before the first frame update
        void OnEnable()
        {
        }

        // Update is called once per frame
        void OnDisable()
        {
        }
    }
}
