using Aci.Unity.Gamification;
using Aci.Unity.Sensor;
using UnityEngine;

namespace Aci.Unity.UI.ViewControllers
{
    public class MouseViewController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;
        
        private AvatarMemory m_AvatarMemory;
        private int m_GreetHash;
        private int m_PensiveHash;
        private int m_SpeakHash;
        private int m_HappyHash;
        private int m_SurprisedHash;


        [Zenject.Inject]
        private void Construct(AvatarMemory avatarMemory)
        {
            m_AvatarMemory = avatarMemory;
        }

        private void Awake()
        {
            m_GreetHash = Animator.StringToHash("Greet");
            m_PensiveHash = Animator.StringToHash("Pensive");
            m_SpeakHash = Animator.StringToHash("Speak");
            m_HappyHash = Animator.StringToHash("Happy");
            m_SurprisedHash = Animator.StringToHash("Surprised");
        }

        private void Start()
        {
            TriggerAnimation();
        }

        private void TriggerAnimation()
        {
            if (m_AvatarMemory.currentKnowledge.HasFlag(AvatarMemory.Knowledge.JustLoggedIn))
                m_Animator.SetTrigger(m_GreetHash);
            else if (m_AvatarMemory.currentKnowledge.HasFlag(AvatarMemory.Knowledge.JustAdvancedStep))
                m_Animator.SetTrigger(m_HappyHash);
            else if (m_AvatarMemory.currentKnowledge.HasFlag(AvatarMemory.Knowledge.JustHadIncorrectStep))
                m_Animator.SetTrigger(Random.Range(0,1) > 0 ? m_PensiveHash : m_SurprisedHash);
            else
                m_Animator.SetTrigger(m_SpeakHash);
        }
    }
}
