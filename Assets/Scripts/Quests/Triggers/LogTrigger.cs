using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public class LogTrigger : QuestTrigger
    {
        [SerializeField]
        private string m_Message;

        public override void Execute()
        {
            Debug.Log(m_Message);
        }
    }
}

