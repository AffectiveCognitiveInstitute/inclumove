using System.Collections.Generic;
using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.Workflow.WorkflowEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LevelHandle : MonoBehaviour, ISubHandle
{
    private SelectionHandle m_ParentHandle;

    private bool m_IsActive = false;

    [SerializeField]
    private RectTransform m_SelectorTransform;
    [SerializeField]
    private List<Slider> m_Sliders = new List<Slider>();

    void Awake()
    {
        m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
        handleActive.AddListener(m_ParentHandle.OnHandleActivated);
        m_SelectorTransform.gameObject.SetActive(false);
    }

    public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

    public void Toggle()
    {
        ToggleActive(!m_IsActive, false);
    }

    public void ToggleActive(bool enabled, bool silent)
    {
        m_IsActive = enabled;
        if(m_IsActive)
            SlidersFromLevels(m_ParentHandle.target.levelable.level);
        m_SelectorTransform.gameObject.SetActive(enabled);
        if (silent)
            return;
        handleActive.Invoke(this, enabled, false);
    }

    public void ToggleHidden(bool hidden)
    {
        gameObject.SetActive(!hidden);
        m_SelectorTransform.gameObject.SetActive(m_IsActive && !hidden);
    }

    public void OnSliderChanged()
    {
        m_ParentHandle.target.levelable.level = LevelsFromSliders();
    }

    private void SlidersFromLevels(byte levels)
    {
        for (int i = 0; i < m_Sliders.Count; ++i)
        {
            m_Sliders[i].value = ((levels & (1 << i)) != 0) ? 1 : 0;
        }
        
    }

    private byte LevelsFromSliders()
    {
        int value = 0;
        for (int i = 0; i < m_Sliders.Count; ++i)
        {
            value |= (int)(m_Sliders[i].value) << i;
        }
        return (byte)value;
    }
}
