// <copyright file=ModuleInstaller.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/24/2018 10:12</date>

using Aci.Unity.Chat;
using Aci.Unity.Data;
using Aci.Unity.UserInterface.Factories;
using Aci.Unity.UserInterface.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class ChatFactoryInstaller : MonoInstaller<ChatFactoryInstaller>
    {
        [SerializeField]
        private GameObject m_HeroCard;

        [SerializeField]
        private GameObject m_TextCard;

        [SerializeField]
        private GameObject m_ActionSheetCard;

        [SerializeField]
        private GameObject m_MaterialCard;

        [SerializeField]
        private GameObject m_VideoCard;

        [SerializeField]
        private GameObject m_MilestoneCard;

        public override void InstallBindings()
        {
            Container.Bind<IAttachmentProviderRegistry>().To<AttachmentProviderRegistry>().AsSingle();
            Container.BindFactory<Object, ChatCardViewController, ChatCardViewController.Factory>().FromFactory<PrefabFactory<ChatCardViewController>>().NonLazy();
            Container.BindInterfacesAndSelfTo<CardContentFactory>().AsSingle();
            //Container.Bind<IFactory<ImageCardViewController>>().FromComponentInHierarchy().AsSingle();
            //Container.Bind<IFactory<TextInputCardViewController>>().FromComponentInHierarchy().AsSingle();
            Container.BindMemoryPool<TextCardViewController, MonoMemoryPool<TextCardViewController>>().FromComponentInNewPrefab(m_TextCard);
            Container.BindMemoryPool<ActionSheetCardViewController, MonoMemoryPool<ActionSheetCardViewController>>().FromComponentInNewPrefab(m_ActionSheetCard);
            Container.BindMemoryPool<HeroCardViewController, MonoMemoryPool<HeroCardViewController>>().FromComponentInNewPrefab(m_HeroCard);
            Container.BindMemoryPool<MaterialCardViewController, MonoMemoryPool<MaterialCardViewController>>().FromComponentInNewPrefab(m_MaterialCard);
            Container.BindFactory<VideoCardViewController, VideoCardViewController.Factory>().FromMonoPoolableMemoryPool(x => x.FromComponentInNewPrefab(m_VideoCard));
            Container.BindFactory<string, MilestoneData, MilestoneAchievedChatViewController, MilestoneAchievedChatViewController.Factory>().FromComponentInNewPrefab(m_MilestoneCard);
        }
    }
}