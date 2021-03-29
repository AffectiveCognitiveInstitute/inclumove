using Aci.Unity.UserInterface.Style;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Milestone Commands/Override Style Attribute Command")]
    public class OverrideStyleAttributeCommand : MilestoneCommand
    {
        [SerializeField]
        private Style m_Style;

        [SerializeField]
        private List<OverridableAttribute> m_Overrides;


        public override void OnBecameActivated()
        {
            foreach(OverridableAttribute attribute in m_Overrides)
            {
                if(m_Style.TryGetAttribute(attribute.attributeName, out OverridableAttribute overridableAttribute))
                {
                    overridableAttribute.OverrideFrom(attribute);
                }
            }
        }

        public override void OnBecameDeactivated()
        {
            foreach (OverridableAttribute attribute in m_Overrides)
            {
                if (m_Style.TryGetAttribute(attribute.attributeName, out OverridableAttribute overridableAttribute))
                {
                    overridableAttribute.isOverriden = false;
                }
            }
        }
    }
}
