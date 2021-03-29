using Aci.Unity.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToggleStyleOverride : MonoBehaviour
{
    [SerializeField]
    private MilestoneCommand m_Command;

    private bool m_IsActive = false;

    public void Execute()
    {
        m_IsActive = !m_IsActive;
        if (m_IsActive)
            m_Command.OnBecameActivated();
        else
            m_Command.OnBecameDeactivated();
    }
}
