using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aci.Unity.Util;

namespace Aci.Unity.Quests
{
    public class USBLogTrigger : QuestTrigger
    {
        [SerializeField]
        private string m_Message;

        public override void Execute()
        {
        }
    }
}

