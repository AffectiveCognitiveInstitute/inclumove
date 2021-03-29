using Aci.Unity.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface
{
    public class MilestoneDialogContentFactory : IFactory<RectTransform, QuestContent, QuestContentViewController>
    {
        private Dictionary<Type, QuestContentViewController.Factory> m_Factories = new Dictionary<Type, QuestContentViewController.Factory>();
        private DiContainer m_DiContainer;

        public MilestoneDialogContentFactory(DiContainer diContainer)
        {
            m_DiContainer = diContainer;
        }

        public QuestContentViewController Create(RectTransform parent, QuestContent content)
        {
            Type type = content.GetType();

            if (m_Factories.TryGetValue(type, out QuestContentViewController.Factory factory))
                return factory.Create(parent, content);

            QuestContentViewController.Factory newFactory = m_DiContainer.TryResolveId<QuestContentViewController.Factory>(type);
            if(newFactory != null)
            {
                m_Factories.Add(type, newFactory);
                return newFactory.Create(parent, content);
            }

            throw new Exception($"Could not find suitable Factory for type {type}.");
        }
    }
}
