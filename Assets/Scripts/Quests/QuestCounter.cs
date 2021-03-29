using UnityEngine;

namespace Aci.Unity.Quests
{
    public delegate void QuestCounterValueChangedDelegate(QuestCounter counter, int previousValue);

    public abstract class QuestCounter : ScriptableObject
    {
        [SerializeField]
        private string m_Id;

        public string id => m_Id;

        [SerializeField]
        private int m_Value;

        public int value
        {
            get => m_Value;
            protected set
            {
                if (m_Value == value)
                    return;

                int oldValue = m_Value;
                m_Value = value;
                valueChanged?.Invoke(this, oldValue);
            }
        }

        public event QuestCounterValueChangedDelegate valueChanged;
        
        private Quest m_Quest;

        public Quest quest => m_Quest;

        public static implicit operator int(QuestCounter counter) => counter.value;

        public void Initialize(Quest quest)
        {
            m_Quest = quest;
        }

        public abstract void StartListening();

        public abstract void StopListening();

        public override string ToString()
        {
            return value.ToString();
        }
    }
}