using Aci.Unity.Gamification;
using Aci.Unity.Quests;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Milestone")]
    public class MilestoneData : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private string m_Title;
        [SerializeField]
        private string m_Subtitle;
        [SerializeField]
        private ColorScheme m_ColorScheme;
        [SerializeField]
        private Quest m_RequiredQuest;
        [SerializeField]
        private Unlockable m_Unlockable;

        /// <summary>
        ///     Unique identifier of milestone.
        /// </summary>
        public string id => m_Id;

        /// <summary>
        ///     The milestone's icon.
        /// </summary>
        public Sprite icon => m_Icon;

        /// <summary>
        ///     The title of this milestone.
        /// </summary>
        public string title => m_Title;

        /// <summary>
        ///     The subtitle of this milestone.
        /// </summary>
        public string subtitle => m_Subtitle;

        /// <summary>
        ///     The quest required to achieve this milestone.
        /// </summary>
        public Quest requiredQuest => m_RequiredQuest;

        /// <summary>
        ///     Does this Milestone contain unlockable content?
        /// </summary>
        public bool hasUnlockableContent => m_Unlockable != null && m_Unlockable.command != null;

        /// <summary>
        ///     Color scheme used for milestone.
        /// </summary>
        public ColorScheme colorScheme => m_ColorScheme;

        /// <summary>
        ///     Contains the unlockable.
        /// </summary>
        public Unlockable unlockable => m_Unlockable;

        [System.Serializable]
        public class Unlockable
        {
            [SerializeField, Tooltip("Contains a list of any milestones that should be deactivated when this unlockable becomes activated.")]
            private List<MilestoneData> m_MilestonesToDeactivate;
            
            [SerializeField, Tooltip("The command containing the logic.")]
            private MilestoneCommand m_Command;
            
            [SerializeField, Tooltip("A preview of the unlockable")]
            private Sprite m_Preview;

            /// <summary>
            /// Contains a list of any milestones that should be deactivated when this unlockable becomes activated.
            /// </summary>
            public IReadOnlyList<MilestoneData> unlocksToDeactivate => m_MilestonesToDeactivate;

            /// <summary>
            /// The command containing the logic.
            /// </summary>
            public MilestoneCommand command => m_Command;

            /// <summary>
            /// Contains a preview of the unlockable.
            /// </summary>
            public Sprite preview => m_Preview;
        }
    }    
}