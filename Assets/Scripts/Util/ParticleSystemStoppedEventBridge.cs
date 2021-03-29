using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Aci.Unity.Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemStoppedEventBridge : MonoBehaviour
    {
        [System.Serializable]
        public class ParticleSystemStoppedEvent : UnityEvent<ParticleSystem> { }

        [SerializeField]
        private ParticleSystemStoppedEvent m_ParticleSystemStopped = new ParticleSystemStoppedEvent();
        private ParticleSystem m_ParticleSystem;

        private List<ParticleSystem> m_ParticleSystems = new List<ParticleSystem>();
        public ParticleSystemStoppedEvent particleSystemStopped => m_ParticleSystemStopped;

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = m_ParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.None;
        }

        public void PlayAll()
        {
            m_ParticleSystem.GetComponentsInChildren(m_ParticleSystems);
            for (int i = 1; i < m_ParticleSystems.Count; i++)
                m_ParticleSystems[i].Play();
            StartCoroutine(Execute());
        }

        public void StopWatching()
        {
            StopCoroutine(Execute());
        }

        private IEnumerator Execute()
        {
            while (!HaveAllParticleSystemsStartedPlaying())
                yield return null;

            while (!HaveAllParticleSystemsCompleted())
                yield return null;

            m_ParticleSystemStopped?.Invoke(m_ParticleSystem);            
        }

        private bool HaveAllParticleSystemsCompleted()
        {
            // We skip the first particle system in the list as this has 0 particles
            // and just triggers playing the child particle systems.
            for (int i = 1; i < m_ParticleSystems.Count; i++) 
            {
                ParticleSystem ps = m_ParticleSystems[i];
                switch (ps.main.startDelay.mode)
                {
                    case ParticleSystemCurveMode.Constant:
                        if (ps.time < ps.main.startDelay.constant)
                            return false;
                        break;
                    case ParticleSystemCurveMode.Curve:
                        if (ps.time < ps.main.startDelay.curve.keys.Max(x => x.value))
                            return false;
                        break;
                    case ParticleSystemCurveMode.TwoCurves:
                        if (ps.time < ps.main.startDelay.curveMin.keys.Max(x => x.value) ||
                            ps.time < ps.main.startDelay.curveMin.keys.Max(x => x.value))
                            return false;
                        break;
                    case ParticleSystemCurveMode.TwoConstants:
                        if (ps.time < ps.main.startDelay.constantMax)
                            return false;
                        break;
                }

                if (ps.particleCount > 0)
                    return false;
            }

            return true;
        }

        private bool HaveAllParticleSystemsStartedPlaying()
        {
            // We skip the first particle system in the list as this has 0 particles
            // and just triggers playing the child particle systems.
            for (int i = 1; i < m_ParticleSystems.Count; i++)
            {
                if (!m_ParticleSystems[i].isPlaying)
                    return false;
            }

            return true;
        }
    }
}