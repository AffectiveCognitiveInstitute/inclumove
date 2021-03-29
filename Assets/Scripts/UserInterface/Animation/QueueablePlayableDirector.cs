using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Aci.Unity.UserInterface.Animation
{
    public class QueueablePlayableDirector : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private PlayableDirector m_Director;

        public async Task Play()
        {
            m_Director.Play();
            await Task.Delay(40);
            await new WaitUntil(() => { return m_Director.state == PlayState.Paused; });
            m_Director.Stop();
        }
    }
}