using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class HeroCardViewController : ImageCardViewController
    {
        [SerializeField]
        private GameObject m_Template;

        [SerializeField]
        private Transform m_ComponentContainer;

        private List<ActionSheetOptionViewController> m_Options = new List<ActionSheetOptionViewController>();

        public void Initialize(string message, 
                                     string imageUrl,
                                     IEnumerable<ActionSheetOption> options)
        {
            this.message = message;
            this.imageUrl = imageUrl;
            
            if (options != null)
            {
                foreach (var option in options)
                    AddOption(option);
            }
            else
            {
                m_ComponentContainer.gameObject.SetActive(false);
            }
        }

        private void AddOption(ActionSheetOption options)
        {
            var go = Instantiate(m_Template, m_ComponentContainer, false);
            var vc = go.GetComponent<ActionSheetOptionViewController>();
            vc.Initialize(options, OnActionSheetOptionClicked);
            m_Options.Add(vc);
        }

        private void OnActionSheetOptionClicked(ActionSheetOptionViewController vc)
        {
            foreach (var o in m_Options)
            {
                if (o == vc)
                    continue;

                o.DisableButton();
            }

            vc.option.Invoke();
        }
    }
}