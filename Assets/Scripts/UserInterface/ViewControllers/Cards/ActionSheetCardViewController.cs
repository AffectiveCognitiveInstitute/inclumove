using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class ActionSheetCardViewController : TextCardViewController
    {
        [SerializeField]
        private Transform m_ActionSheetContainer;

        [SerializeField]
        private GameObject m_ActionSheetTemplate;

        private List<ActionSheetOptionViewController> m_Options = new List<ActionSheetOptionViewController>();

        public void Initialize(IEnumerable<ActionSheetOption> options, string message)
        {
            foreach (var option in options)
                AddOption(option);

            this.message = message;
        }

        private void AddOption(ActionSheetOption option)
        {
            var instance = Instantiate(m_ActionSheetTemplate, m_ActionSheetContainer, false);
            ActionSheetOptionViewController vc = instance.GetComponent<ActionSheetOptionViewController>();
            vc.Initialize(option, OnActionSheetOptionClicked);
            m_Options.Add(vc);
        }

        private void OnActionSheetOptionClicked(ActionSheetOptionViewController vc)
        {
            foreach(var o in m_Options)
            {
                if (o == vc)
                    continue;

                o.DisableButton();
            }

            vc.option.Invoke();
        }

        public class Factory : PlaceholderFactory<ActionSheetCardViewController> { }
    }
}