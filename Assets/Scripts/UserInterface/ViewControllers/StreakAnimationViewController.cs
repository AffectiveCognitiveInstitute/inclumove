using UnityEngine;
using System.Collections;
using Aci.Unity.UI.Tweening;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class StreakAnimationViewController : MonoBehaviour
                                               , IQueueableAnimation
                                               , IAciEventHandler<WorkflowLoadArgs>
    {
        [SerializeField]
        private TweenerDirector m_FadeInDirector;

        [SerializeField]
        private QueueableTweenerDirector m_FadeInDirectorAnimation;

        [SerializeField]
        private TweenerDirector m_FadeOutDirector;

        [SerializeField]
        private QueueableTweenerDirector m_FadeOutDirectorAnimation;

        [SerializeField]
        private GameObject m_StreakParticleBadgePrefab;

        private IAciEventManager m_EventManager;
        private IBadgeService m_BadgeService;
        private IAnimationQueue m_AnimationQueue = new AnimationQueue();

        private void Start()
        {
            RegisterForEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        [Zenject.Inject]
        public void Construct(IAciEventManager eventManager, IBadgeService badgeService)
        {
            m_EventManager = eventManager;
            m_BadgeService = badgeService;
        }


        public async void OnEvent(WorkflowLoadArgs arg)
        {
            // remove all previous badges
            while (transform.childCount > 0)
            {
                Transform trans = transform.GetChild(0);
                trans.parent = null;
                Destroy(trans.gameObject);
            }
            await new WaitForEndOfFrame();
            for (int i = m_BadgeService.streakLevels[2]; i > 0; --i)
            {
                GameObject obj = GameObject.Instantiate(m_StreakParticleBadgePrefab);
                obj.transform.SetParent(transform, false);
                obj.transform.localScale = Vector3.one;
            }
        }

        public void SetBadgeTier(int tier)
        {
            m_AnimationQueue.Clear();
            m_FadeInDirector.RemoveTweenersAfter(-1);
            m_FadeOutDirector.RemoveTweenersAfter(-1);
            m_AnimationQueue.Enqueue(m_FadeInDirectorAnimation);
            QueueableAnimationSynchronizer synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
            for (int i = 0; i < m_BadgeService.streakLevels[tier]; ++i)
            {
                Transform trans = transform.GetChild(i);
                trans.gameObject.SetActive(true);
                StreakParticleBadgeViewController viewController = trans.GetComponent<StreakParticleBadgeViewController>();
                viewController.SetBadgeTier(tier);
                viewController.blendInTweener.delayTime = 0.2f * i;
                m_FadeInDirector.AddTweener(viewController.blendInTweener);
                m_FadeOutDirector.AddTweener(viewController.blendOutTweener);
                synchedAnim.Append(viewController.queueableParticleAnimation);
            }
            for (int i = m_BadgeService.streakLevels[tier]; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            m_AnimationQueue.Enqueue(synchedAnim);
            m_AnimationQueue.Enqueue(m_FadeOutDirectorAnimation);
        }

        public async Task Play()
        {
            await m_AnimationQueue.Play();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }
    }
}
