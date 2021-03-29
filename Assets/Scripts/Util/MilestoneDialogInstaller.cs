using Aci.Unity.Data;
using Aci.Unity.UserInterface.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class MilestoneDialogInstaller : MonoInstaller<MilestoneDialogInstaller>
    {
        [SerializeField]
        private GameObject m_DialogPrefab;

        [SerializeField]
        private Transform m_Parent;

        public override void InstallBindings()
        {
            Container.BindFactory<MilestoneData, MilestoneDialogueViewController, MilestoneDialogueViewController.Factory>().
                      FromComponentInNewPrefab(m_DialogPrefab).UnderTransform(m_Parent);
        }
    }
}