using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Workflow;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification
{
    public class TimelineItemFactory : IFactory<WorkflowStepData, ITimelineItemViewController>
    {
        private TimelineItemPrefabRepository m_PrefabRepository;

        [Zenject.Inject]
        private void Construct(TimelineItemPrefabRepository prefabRepo)
        {
            m_PrefabRepository = prefabRepo;
        }

        /// <inheritdoc />
        public ITimelineItemViewController Create(WorkflowStepData param)
        {
            ITimelineItemViewController controller = null;
            if (param.automatic)
            {
                controller = GameObject.Instantiate(m_PrefabRepository.automaticPrefab).GetComponent<ITimelineItemViewController>();
            }
            else
            {
                controller = GameObject.Instantiate(m_PrefabRepository.progressPrefab).GetComponent<ITimelineItemViewController>();
            }

            return controller;
        }
    }
}