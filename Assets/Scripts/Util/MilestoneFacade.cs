using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Quests;
using Aci.Unity.UI.Dialog;
using Aci.Unity.UserInterface.ViewControllers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class MilestoneFacade : IMilestoneFacade
    {
        private readonly IDialogService m_DialogService;
        private readonly MilestoneAchievedViewController.Factory m_PopupFactory;
        private readonly MilestoneDataCollection m_Milestones;
        private readonly IQuestFacade m_QuestFacade;
        private readonly IUserManager m_UserManager;
        private readonly DiContainer m_DiContainer;

        public MilestoneFacade(IDialogService dialogService, 
                               MilestoneAchievedViewController.Factory popupFactory,
                               MilestoneDataCollection milestones,
                               IQuestFacade questFacade,
                               IUserManager userManager,
                               DiContainer diContainer)
        {
            m_DialogService = dialogService;
            m_PopupFactory = popupFactory;
            m_Milestones = milestones;
            m_QuestFacade = questFacade;
            m_UserManager = userManager;
            m_DiContainer = diContainer;
        }

        /// <inheritdoc/>
        public void ActivateUnlockable(string id, bool storeChanges = true)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            MilestoneData data = m_Milestones.GetDataById(id);

            if (data == null)
            {
                Debug.LogError($"Milestone with id {id} does not exist. Could not activate unlockable!");
                return;
            }

            if (!data.hasUnlockableContent)
            {
                Debug.LogError($"Milestone with id {id} does not have unlockable content.");
                return;
            }

            IReadOnlyList<MilestoneData> unlocksToDeactivate = data.unlockable.unlocksToDeactivate;
            for (int i = 0; i < unlocksToDeactivate.Count; i++)
                m_UserManager.CurrentUser.RemoveActivatedMilestone(unlocksToDeactivate[i].id);

            MilestoneCommand command = UnityEngine.Object.Instantiate(data.unlockable.command);
            m_DiContainer.Inject(command);
            command.OnBecameActivated();
            m_UserManager.CurrentUser.AddActivatedMilestone(data.id);

            if (storeChanges)
                m_UserManager.SaveUser();
        }

        /// <inheritdoc/>
        public void DeactivateUnlockable(string id, bool storeChanges = true)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            MilestoneData data = m_Milestones.GetDataById(id);

            if (data == null)
            {
                Debug.LogError($"Milestone with id {id} does not exist. Could not deactivate unlockable!");
                return;
            }

            if (data.hasUnlockableContent)
            {
                MilestoneCommand command = UnityEngine.Object.Instantiate(data.unlockable.command);
                m_DiContainer.Inject(command);
                command.OnBecameDeactivated();
            }

            m_UserManager.CurrentUser.RemoveActivatedMilestone(data.id);

            if (storeChanges)
                m_UserManager.SaveUser();
        }

        /// <inheritdoc/>
        public IDialog DisplayMilestoneAchieved(MilestoneData milestoneData)
        {
            DialogComponent dialogComponent = m_PopupFactory.Create(milestoneData);
            DialogRequest request = DialogRequest.Create(dialogComponent, DialogPriority.Low);
            m_DialogService.SendRequest(request);
            return dialogComponent;
        }

        /// <inheritdoc/>
        public void StartMilestoneQuests()
        {
            foreach(MilestoneData milestoneData in m_Milestones)
            {
                try
                {
                    if (ReferenceEquals(milestoneData, null))
                        continue;

                    if (milestoneData.requiredQuest != null)
                        m_QuestFacade.StartQuest(milestoneData.requiredQuest);

                    if (m_UserManager.CurrentUser.IsMilestoneActivated(milestoneData.id))
                        ActivateUnlockable(milestoneData.id, false);
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <inheritdoc/>
        public void StopMilestoneQuests()
        {
            foreach (MilestoneData milestoneData in m_Milestones)
            {
                try
                {
                    if (ReferenceEquals(milestoneData, null))
                        continue;

                    if (milestoneData.requiredQuest != null)
                        m_QuestFacade.StopQuest(milestoneData.requiredQuest.id);
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}