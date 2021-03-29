using Aci.Unity.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aci.Unity.UserInterface.Style
{
    public class StyleBinder : MonoBehaviour
    {
        [SerializeField]
        private StyleReference m_Style;

        [SerializeField]
        private MonoBehaviour m_Target;

        [SerializeField]
        private string m_AttributeName;

        [SerializeField]
        private string m_PropertyName;

        [SerializeField]
        private bool m_UpdateInEditor;

        public Style style => m_Style;

        private OverridableAttribute m_SourceAttribute;

        private PropertyInfo m_TargetPropertyInfo;

        private void Awake()
        {
            m_SourceAttribute = m_Style.value[m_AttributeName];

            if (Application.isPlaying)
            {
                Assert.IsFalse(string.IsNullOrEmpty(m_AttributeName), "The attribute name cannot be null");
                UpdateProperty();
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                m_SourceAttribute.valueChanged += OnAttributeValueChanged;
                UpdateProperty();
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
#endif
                m_SourceAttribute.valueChanged -= OnAttributeValueChanged;
        }

        private void OnAttributeValueChanged(object sender, AttributeValueChangedEventArgs eventArgs)
        {
            UpdateProperty();
        }
        
        private void UpdateProperty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !m_UpdateInEditor)
                return;
#endif
            if (m_Style == null)
                return;

            if (string.IsNullOrWhiteSpace(m_AttributeName))
                return;

            if (m_Target == null)
                return;

            if(m_TargetPropertyInfo == null)
                m_TargetPropertyInfo = m_Target.GetType().GetProperties().FirstOrDefault(x => x.Name == m_PropertyName);
                       
            if(m_TargetPropertyInfo != null)
            {
                try
                {
                    m_TargetPropertyInfo.SetValue(m_Target, m_Style.value[m_AttributeName].Get());
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateProperty();
        }
#endif
    }
}