// <copyright file=EventTypes.cs/>
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
// <date>07/12/2018 11:06</date>

using System;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Models;
using Aci.Unity.Scene.SceneItems;

namespace Aci.Unity.Events
{
    public struct UserArgs
    {
        public enum UserEventType
        {
            Login,
            Logout,
            Save
        };
        public UserEventType eventType;
    }

    public struct WorkflowLoadArgs
    {
        public string msg;
    }

    public struct WorkflowStartArgs
    {
        public string msg;
    }

    public struct WorkflowStopArgs
    {
        public float time;
    }

    public struct WorkflowStepFinalizedArgs
    {
        public int lastStep;
        public int lastDataStep;
        public int newStep;
        public int newDataStep;
        public int executedRepetitions;
    }

    public struct WorkflowStepEndedArgs
    {
        public int lastStep;
        public int lastDataStep;
        public int newStep;
        public int newDataStep;
        public int executedRepetitions;
    }

    public struct ChatContextArgs
    {
        public string context;
    }

    public struct ChatAnswerArgs
    {
        public string msg;
    }

    public struct ChatInputArgs
    {
        public string msg;
    }

    public struct CameraStatusArgs
    {
        public bool active;
    }

    public struct ActionsChangedArgs
    {
        public BotAction[] actions;
    }

    public struct DemoResetArgs
    {
    }

    public struct UserDetectedArgs
    {
    }

    public struct EmotionChanged
    {
        public int newEmotion;
    }

    public struct SelectionChanged
    {
        public ISelectable previous;
        public ISelectable current;
    }

    public struct USBDevice
    {
        public string drive;
    }

    public struct MilestoneAchievedArgs
    {
        public MilestoneData milestone;
    }

    public struct CVTriggerArgs
    {
        public int id;
        public bool okay;
    }

    public struct AdaptivityLevelChangeRepliedArgs
    {
        public bool wasChanged;
    }

    public struct WorkingDayCountChangedEvent
    {
        public int PrevCount;
        public int NewCount;
    }

    public struct WorkingHourCountChangedEvent
    {
        public float SpentTime;
    }

    public struct BadgeAmountCountChangedEvent
    {
        public int NewCount;
        public int TotalCount;
    }


}