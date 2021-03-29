// <copyright file=BotConnect.cs/>
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
// <date>08/02/2018 16:11</date>

using System;
using System.Linq;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Models;
using Aci.Unity.Scene;
using Aci.Unity.UI.Localization;
using Aci.Unity.Workflow;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Bot
{
    /// <summary>
    ///     Notifies a bot about changes of the workflow.
    /// </summary>
    [Obsolete("Use IBotMessenger interface to send messages. Events are handled by other classes")]
    public class BotConnect : MonoBehaviour
                            , IAciEventHandler<UserLoginArgs>
                            , IAciEventHandler<WorkflowLoadArgs>
                            , IAciEventHandler<WorkflowStepFinalizedArgs>
                            , IAciEventHandler<WorkflowStartArgs>
                            , IAciEventHandler<WorkflowStopArgs>
                            , IAciEventHandler<LocalizationChangedArgs>
    {

        private const string clientFinishedWorkflowStep = nameof(clientFinishedWorkflowStep);
        private const string clientReadyForWorkflow = nameof(clientReadyForWorkflow);
        private const string workflowAvailable = nameof(workflowAvailable);

        private IAciEventManager __broker;

        private IBot bot;

        [Inject]
        private IWorkflowService m_WorkflowService;

        [Inject]
        private WorkflowLoader loader;

        [Inject]
        private IUserManager userManager;

        [Inject]
        private ILocalizationManager locMan;

        private int lastWorkStep = -1;
        private string locale = "de";


        [Inject]
        private IAciEventManager broker
        {
            get { return __broker; }
            set
            {
                if (value == null)
                    return;
                UnregisterFromEvents();
                __broker = value;
                RegisterForEvents();
            }
        }

        public void OnEvent(LocalizationChangedArgs args)
        {
        }

        public void RegisterForEvents()
        {
            //broker?.AddHandler<UserLoginArgs>(this);
            //broker?.AddHandler<WorkflowStepArgs>(this);
            //broker?.AddHandler<WorkflowLoadArgs>(this);
            //broker?.AddHandler<WorkflowStartArgs>(this);
            //broker?.AddHandler<WorkflowStopArgs>(this);
            //broker?.AddHandler<LocalizationChangedArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            //broker?.RemoveHandler<UserLoginArgs>(this);
            //broker?.RemoveHandler<WorkflowStepArgs>(this);
            //broker?.RemoveHandler<WorkflowLoadArgs>(this);
            //broker?.RemoveHandler<WorkflowStartArgs>(this);
            //broker?.RemoveHandler<WorkflowStopArgs>(this);
            //broker?.RemoveHandler<LocalizationChangedArgs>(this);
        }

        public void OnEvent(UserLoginArgs args)
        {
            // setup bot
            bot.Account.Id = GenerateUniqueId();
            bot.StartConversation();
        }

        public void OnEvent(WorkflowLoadArgs args)
        {
            SendEventToBot(clientReadyForWorkflow, args.msg);
        }

        public void OnEvent(WorkflowStartArgs args)
        {
            // TODO: this is currently not used in the IFA-demonstrator since the intermediate json of the bot doesnt discern between workflow loading and starting, FIX THIS AFTER!
            // SendMessageToBot("event", workflowStart, "workflow_started", false);
            lastWorkStep = -1;
        }

        public void OnEvent(WorkflowStepFinalizedArgs args)
        {
            if (lastWorkStep == args.newStep)
                return;
            lastWorkStep = args.newStep;
            
            SendEventToBot(clientFinishedWorkflowStep, m_WorkflowService.currentWorkflowData.steps[args.newStep].id);
        }

        public void OnEvent(WorkflowStopArgs args)
        {
        }

        ~BotConnect()
        {
            Teardown();
        }

        public void Teardown()
        {
            UnregisterFromEvents();
        }

        private void Start()
        {
            bot = GetComponent<IBot>();
            bot.SystemActivityReceived.AddListener(HandleActivity);
        }

        public void SendMessageToBot(string text)
        {
            var activity = new Activity
            {
                Type = "message",
                Name = name,
                Text = text,
                //Value = text,
                Locale = locale
            };

            bot.SendActivityAsync(activity);
        }

        public void SendEventToBot(string name, object value = null)
        {
            var activity = new Activity
            {
                Type = "event",
                Name = name,
                Value = value,
                Locale = locale
            };

            bot.SendActivityAsync(activity);
        }
        
        private void ReactOnActivity(Activity activity)
        {
            switch (activity.Type)
            {
                case "event":
                    HandleActivity(activity);
                    break;
            }

            if (activity.SuggestedActions?.Actions?.Count > 0)
            {
                var actions = activity.SuggestedActions.Actions
                    .Select(cardAction => new BotAction
                    {
                        title = cardAction.Title,
                        type = cardAction.Type,
                        value = cardAction.Value.ToString()
                    })
                    .ToArray();

                __broker.Invoke(new ActionsChangedArgs() { actions = actions });
            }
        }

        private void HandleActivity(Activity activity)
        {
            switch (activity.Name)
            {
                case workflowAvailable:
                    loader.LoadWorkflow(activity.Value + ".work");
                    break;
            }
        }

        private string GenerateUniqueId()
        {
            var epoch = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            var timestamp = (DateTime.UtcNow - epoch).TotalSeconds;
            var random = new System.Random();

            return string.Format("{0:X}{1:X}", Convert.ToInt32(timestamp), random.Next(1000000000));
        }
    }
}