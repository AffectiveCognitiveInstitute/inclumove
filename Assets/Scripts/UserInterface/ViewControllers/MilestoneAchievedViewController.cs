using System;
using System.Collections;
using Aci.UI.Binding;
using Aci.Unity.Data;
using Aci.Unity.UI.Dialog;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(DialogComponent))]
    public class MilestoneAchievedViewController : MonoBindable
    {
        public class Factory : PlaceholderFactory<MilestoneData, MilestoneAchievedViewController> { }

        [SerializeField]
        private float m_DisplayTime = 5f;

        [SerializeField]
        private MaskableGraphic[] m_Hexagons;

        private string m_Title;
        public string title
        {
            get => m_Title;
            set => SetProperty(ref m_Title, value);
        }

        private Sprite m_Icon;
        public Sprite icon
        {
            get => m_Icon;
            set => SetProperty(ref m_Icon, value);
        }

        private DialogComponent m_DialogComponent;

        public DialogComponent dialogComponent
        {
            get
            {
                if(m_DialogComponent == null)
                    m_DialogComponent = GetComponent<DialogComponent>();

                return m_DialogComponent;
            }
        }
        
        [Zenject.Inject]
        private void Construct(MilestoneData milestoneData)
        {
            title = milestoneData.title;
            icon = milestoneData.icon;
        }

        private void Awake()
        {
            dialogComponent.dialogAppeared.AddListener(OnDialogAppeared);
            dialogComponent.dismissed += OnDismissed;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.04f);
            for (int i = 0; i < m_Hexagons.Length; i++)
                m_Hexagons[i].SetAllDirty();
        }

        private void OnDestroy()
        {
            dialogComponent.dialogAppeared.RemoveListener(OnDialogAppeared);
            dialogComponent.dismissed -= OnDismissed;
        }

        private void OnDialogAppeared(DialogComponent dialogComponent)
        {
            StartCoroutine(DismissAfterDelay());
        }

        private IEnumerator DismissAfterDelay()
        {
            yield return new WaitForSeconds(m_DisplayTime);
            m_DialogComponent.Dismiss();
        }

        private void OnDismissed(IDialog dialog)
        {
            Destroy(gameObject);
        }

        public static implicit operator DialogComponent(MilestoneAchievedViewController vc)
        {
            return vc.dialogComponent;
        }
    }
}