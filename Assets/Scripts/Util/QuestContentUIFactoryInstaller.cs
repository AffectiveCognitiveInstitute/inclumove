using Aci.Unity.Quests;
using Aci.Unity.UserInterface;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    [CreateAssetMenu(menuName = "ACI/Installers/QuestContentUIFactoryInstaller")]
    public class QuestContentUIFactoryInstaller : ScriptableObjectInstaller<QuestContentUIFactoryInstaller>
    {
        [SerializeField]
        private GameObject m_ProgressPrefab;

        [SerializeField]
        private GameObject m_HasCompletedQuestPrefab;

        public override void InstallBindings()
        {
            Container.Bind<MilestoneDialogContentFactory>().ToSelf().AsSingle();

            Container.BindFactory<RectTransform, QuestContent, QuestContentViewController, QuestContentViewController.Factory>().
                      WithId(typeof(QuestProgressContent)).
                      FromComponentInNewPrefab(m_ProgressPrefab);

            Container.BindFactory<RectTransform, QuestContent, QuestContentViewController, QuestContentViewController.Factory>().
                      WithId(typeof(HasCompletedQuestNodeContent)).
                      FromComponentInNewPrefab(m_HasCompletedQuestPrefab);

        }
    }
}