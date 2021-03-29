using System;
using System.Globalization;
using Aci.Unity.Scene.SceneItems;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class StepItemElementViewController : MonoBehaviour
    {
        private WorkflowEditorPanel m_ParentPanel;

        private IStepItem m_DataItem;

        [SerializeField]
        private TMP_InputField m_StepLabel;

        [SerializeField]
        private Toggle m_UnifiedDurationsToggle;

        [SerializeField]
        private TMP_InputField m_StepDurationMin;

        [SerializeField]
        private TMP_InputField m_StepDurationRed;

        [SerializeField]
        private TMP_InputField m_StepDurationMax;

        [SerializeField]
        private TMP_InputField m_StepRepetitions;

        [SerializeField]
        private TMP_InputField m_CameraTriggerId;

        [SerializeField]
        private Toggle m_AutomaticToggle;

        [SerializeField]
        private Toggle m_CameraToggle;

        [SerializeField]
        private Image m_TopBar;

        [SerializeField]
        private Color m_TopBarSelected;

        [SerializeField]
        private Color m_TopBarDeselected;

        private IFormatProvider m_Format;

        public void Inititalize(WorkflowEditorPanel parentPanel, IStepItem item)
        {
            m_Format = new CultureInfo("en");
            m_ParentPanel = parentPanel;
            m_DataItem = item;
            m_StepLabel.text = m_DataItem.name;
            m_StepDurationMin.text = m_DataItem.duration.x.ToString(m_Format);
            m_StepDurationRed.text = m_DataItem.duration.y.ToString(m_Format);
            m_StepDurationMax.text = m_DataItem.duration.z.ToString(m_Format);
            m_UnifiedDurationsToggle.SetIsOnWithoutNotify(m_DataItem.duration.x == m_DataItem.duration.y 
                && m_DataItem.duration.y == m_DataItem.duration.z);
            m_StepRepetitions.text = m_DataItem.repetitions.ToString(m_Format);
            m_AutomaticToggle.isOn = m_DataItem.automatic;
            m_CameraTriggerId.text = m_DataItem.triggerId.ToString(m_Format);
            m_CameraToggle.isOn = m_DataItem.triggerId != -1;
        }

        private void OnDeleteButtonPressed()
        {
            m_ParentPanel.Unregister(this);
        }

        public void OnSelected()
        {
            m_ParentPanel.SetActive(this);
            m_TopBar.color = m_TopBarSelected;
        }

        public void OnActiveChanged(bool active)
        {
            m_TopBar.color = active ? m_TopBarSelected : m_TopBarDeselected;
        }

        public void OnStepLabelChanged(string label)
        {
            if (m_DataItem.name == label)
                return;
            m_DataItem.name = label;
        }

        public void OnDurationLabelChanged(string label)
        {
            float3 duration = new float3(
                  float.Parse(m_StepDurationMin.text, NumberStyles.Float, m_Format)
                , float.Parse(m_StepDurationRed.text, NumberStyles.Float, m_Format)
                , float.Parse(m_StepDurationMax.text, NumberStyles.Float, m_Format));
            if (m_DataItem.duration.Equals(duration) || duration.x < 0 || duration.y < 0 || duration.z < 0)
                return;
            if (m_UnifiedDurationsToggle.isOn)
            {
                duration.x = duration.y = duration.z;
                m_StepDurationMin.SetTextWithoutNotify(duration.x.ToString(m_Format));
                m_StepDurationRed.SetTextWithoutNotify(duration.y.ToString(m_Format));
                m_StepDurationMax.SetTextWithoutNotify(duration.z.ToString(m_Format));
            }
            m_DataItem.duration = duration;
        }

        public void OnUnifiedDurationsToggleChanged(bool activated)
        {
            m_StepDurationMin.interactable = !activated;
            m_StepDurationRed.interactable = !activated;
        }

        public void OnRepetitionsLabelChanged(string label)
        {
            int repetitions = int.Parse(label, m_Format);
            if (m_DataItem.repetitions == repetitions || repetitions < 0)
                return;
            m_DataItem.repetitions = repetitions;
        }

        public void OnAutomaticToggleChanged(bool activated)
        {
            if (m_DataItem.automatic == activated)
                return;
            m_DataItem.automatic = activated;
            if (activated)
            {
                m_CameraToggle.isOn = false;
            }
        }

        public void OnCameraToggleChanged(bool activated)
        {
            m_DataItem.triggerId = int.Parse(m_CameraTriggerId.text, m_Format);
            if (activated)
            {
                m_AutomaticToggle.isOn = false;
                m_CameraTriggerId.interactable = true;
            }
            else
            {
                m_DataItem.triggerId = -1;
                m_CameraTriggerId.interactable = false;
            }
        }

        public void OnCameraTriggerIdLabelChanged(string label)
        {
            int triggerId = int.Parse(label, m_Format);
            if (m_DataItem.triggerId == triggerId)
                return;
            m_DataItem.triggerId = triggerId;
        }
    }
}
