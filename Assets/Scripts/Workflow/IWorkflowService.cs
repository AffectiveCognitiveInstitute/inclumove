using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aci.Unity.Bot;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Logging;
using Aci.Unity.Models;
using Aci.Unity.Network;
using Aci.Unity.Scene;
using Aci.Unity.Sensor;
using Aci.Unity.Util;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow
{
    public interface IWorkflowService
    {
        WorkflowData currentWorkflowData { get; }
        int currentDataStep { get; }
        int currentStep { get; }
        int currentRepetition { get; }
        float currentTime { get; }
        bool isRunning { get; }

        void SetWorkflowData(WorkflowData data);

        void StartWork();

        void StopWork();

        void SetStep(int step);

        void RegisterFinalizer(IStepFinalizer finalizer);

        void UnregisterFinalizer(IStepFinalizer finalizer);
    }
}
