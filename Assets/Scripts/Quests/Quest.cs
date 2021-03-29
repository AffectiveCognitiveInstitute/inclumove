using UnityEngine;

namespace Aci.Unity.Quests
{
    [CreateAssetMenu(menuName = "ACI/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField]
        private string m_Id;

        [SerializeField]
        private QuestNode m_QuestNode;

        [SerializeField, Tooltip("Contains a list of shared variables for a quest")]
        private QuestCounterCollection m_QuestCounters; // Could be just changed to a blackboard for more generic use. For now this will do.

        [SerializeField, Tooltip("Contains content for UI")]
        private QuestContentCollection m_QuestContent;

        private QuestState m_State;

        public string id => m_Id;

        public QuestCounterCollection counters => m_QuestCounters;

        public QuestContentCollection contents => m_QuestContent;

        /// <summary>
        ///     Gets the quest node's state.
        /// </summary>
        public QuestState state
        {
            // Since we only have one quest node at the moment we can just return that
            // QuestNode's state.
            get
            {
                switch (m_QuestNode.state)
                {
                    case QuestNodeState.Inactive:
                        return QuestState.Inactive;
                    case QuestNodeState.Active:
                        return QuestState.Active;
                    case QuestNodeState.Succeeded:
                        return QuestState.Success;
                    case QuestNodeState.Failed:
                        return QuestState.Failed;
                }

                return QuestState.Inactive;
            }
        }

        /// <summary>
        ///     Gets the quest's start node.
        /// </summary>
        public QuestNode startNode => m_QuestNode;


        private void Initialize()
        {
            m_QuestNode.Initialize(this);
            m_QuestCounters.Initialize(this);
            m_QuestContent.Initialize(this);
        }

        /// <summary>
        /// Creates an instance of the Quest.
        /// </summary>
        /// <returns>Returns the instance of the quest.</returns>
        public Quest Instantiate()
        {
            Quest instance = Instantiate(this);
            instance.Initialize();

            return instance;
        }

        /// <summary>
        /// Gets a quest counter by id.
        /// </summary>
        /// <param name="id">The id of the quest id.</param>
        /// <returns>Returns the quest counter.</returns>
        public QuestCounter GetCounterById(string id)
        {
            return m_QuestCounters[id];
        }

        /// <summary>
        ///     Sets the quest's state.
        /// </summary>
        /// <param name="state">The state to be set.</param>
        public void SetState(QuestState state)
        {
            if (m_State == state)
                return;

            m_State = state;
            switch (m_State)
            {
                case QuestState.Inactive:
                    m_QuestNode.SetState(QuestNodeState.Inactive);
                    m_QuestCounters.StopListening();
                    break;
                case QuestState.Active:
                    m_QuestNode.SetState(QuestNodeState.Active);
                    m_QuestCounters.StartListening();
                    break;
                case QuestState.Success:
                    m_QuestNode.SetState(QuestNodeState.Succeeded);
                    m_QuestCounters.StopListening();
                    break;
                case QuestState.Failed:
                    m_QuestNode.SetState(QuestNodeState.Failed);
                    m_QuestCounters.StopListening();
                    break;
            }
        }

    }

    public enum QuestState
    {
        Inactive,
        Active,
        Success,
        Failed
    }
}
