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
        private TMP_InputField m_PartId;

        [SerializeField]
        private TMP_InputField m_GripperId;

        [SerializeField]
        private TMP_Dropdown m_StepTypeDropdown;

        [SerializeField]
        private CanvasGroup m_IdGroup;

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
            m_PartId.text = m_DataItem.partId.ToString(m_Format);
            m_GripperId.text = m_DataItem.gripperId.ToString(m_Format);
            m_StepTypeDropdown.SetValueWithoutNotify(GetDropdownValue());
            if(m_StepTypeDropdown.value != 4)
                m_IdGroup.alpha = 0;
        }

        private int GetDropdownValue()
        {
            if (m_DataItem.automatic)
                return 0;
            if (m_DataItem.mount)
                return 1;
            if (m_DataItem.unmount)
                return 2;
            if (m_DataItem.control)
                return 3;
            return 4;
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

        public void OnDropdownValueChanged(int value)
        {
            m_DataItem.automatic = false;
            m_DataItem.mount = false;
            m_DataItem.unmount = false;
            m_DataItem.control = false;
            m_DataItem.partId = -1;
            m_DataItem.gripperId = -1;
            m_PartId.text = m_DataItem.partId.ToString(m_Format);
            m_GripperId.text = m_DataItem.gripperId.ToString(m_Format);
            m_IdGroup.alpha = 0;

            switch (value)
            {
                case 0:
                    m_DataItem.automatic = true;
                    break;
                case 1:
                    m_DataItem.mount = true;
                    break;
                case 2:
                    m_DataItem.unmount = true;
                    break;
                case 3:
                    m_DataItem.control = true;
                    break;
                case 4:
                    m_DataItem.partId = 0;
                    m_DataItem.gripperId = 0;
                    m_PartId.text = m_DataItem.partId.ToString(m_Format);
                    m_GripperId.text = m_DataItem.gripperId.ToString(m_Format);
                    m_IdGroup.alpha = 1;
                    break;
            }
        }

        public void OnPartIdLabelChanged(string label)
        {
            if (m_StepTypeDropdown.value != 4)
                return;

            int id = int.Parse(label, m_Format);
            if (m_DataItem.partId == id)
                return;
            m_DataItem.partId = id;
        }

        public void OnGripperIdLabelChanged(string label)
        {
            if (m_StepTypeDropdown.value != 4)
                return;

            int id = int.Parse(label, m_Format);
            if (m_DataItem.gripperId == id)
                return;
            m_DataItem.gripperId = id;
        }
    }
}
