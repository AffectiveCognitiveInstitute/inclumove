using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Aci.Unity.Audio
{
    [CreateAssetMenu(menuName = "Inclumove/AudioServiceSettings")]
    public class AudioServiceSettings : ScriptableObject
    {
        [SerializeField]
        private AudioMixer m_Mixer;

        [SerializeField]
        private AudioChannelMixerDictionary m_Channels;

        [SerializeField]
        private AudioChannelCapacityDictionary m_Capacities;

        public AudioMixer mixer => m_Mixer;

        public AudioChannelMixerDictionary channels => m_Channels;
        
        public AudioChannelCapacityDictionary capacities => m_Capacities;
    }
}
