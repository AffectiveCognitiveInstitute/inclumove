using Aci.Unity.Events;
using Aci.Unity.Logging;
using Aci.Unity.Models;
using Aci.Unity.Util;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.Networking
{
    public class MQTTComponents
    {
        public static string Control = "control";
        public static string Mouse = "mouse";
        public static string Table = "table";
        public static string Inspection = "inspection";
        public static string Part_cam = "part_cam";
        public static string Board_cam = "board_cam";
        public static string Live_cam = "live_cam";
        public static string All = "ALL";
    }

    public class MQTTTopics
    {
        public static string Status = "Status";
        public static string Request = "Request";
        public static string Ack = "Ack";
        public static string Response = "Response";
    }

    public enum MQTTStatus : uint
    {
        Offline = 0,
        Ready,
        Busy,
        Error
    }

    public class MQTTConnector : MonoBehaviour
                               , IMqttClientConnectedHandler
                               , IMqttClientDisconnectedHandler
                               , IMqttApplicationMessageReceivedHandler
    {
        private IAciEventManager m_EventManager;
        private IMqttClientFactory m_Factory;
        private IMqttClient m_Client;
        private IMqttClientOptions m_ClientOptions;

        private uint m_RequestIdCounter = uint.MinValue;
        private readonly uint m_ComponentMask = ((uint)1 << 29) & uint.MaxValue;
        private readonly uint m_SubtractionMask = ((uint)1 << 31) | ((uint)1 << 30) | ((uint)1 << 29) ^ uint.MaxValue;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager)
        {
            m_EventManager = eventManager;
        }

        private async void Start()
        {
            m_Factory = new MqttFactory();
            m_Client = m_Factory.CreateMqttClient();

            string json = JsonUtility.ToJson(new Status() { state = (uint)MQTTStatus.Offline, error = "" });
            MqttApplicationMessage lastWillMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"{MQTTComponents.Control}/{MQTTTopics.Status}")
                .WithPayload(json)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            m_ClientOptions = new MqttClientOptionsBuilder()
             .WithClientId("incluMOVE-GUI")
             .WithTcpServer("localhost", 1883)
             .WithWillMessage(lastWillMessage)
             .Build();

            Aci.Unity.Logging.AciLog.Log("Network", "Connecting to Mqtt-Server.");
            try
            {
                m_Client.ConnectAsync(m_ClientOptions, CancellationToken.None);
            }
            catch(SocketException e)
            {
                Aci.Unity.Logging.AciLog.Log("Network", $"Failed to find MQTT-Broker, failed with Exception {e.SocketErrorCode}, {e.Message}");
                m_Client = null;
                return;
            }
            Aci.Unity.Logging.AciLog.Log("Network", "Connected to Mqtt-Server.");

            m_Client.UseDisconnectedHandler(HandleDisconnectedAsync);
            m_Client.UseApplicationMessageReceivedHandler(HandleApplicationMessageReceivedAsync);
            m_Client.UseConnectedHandler(HandleConnectedAsync);
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string json = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
            string topic = eventArgs.ApplicationMessage.Topic;
            int index = topic.LastIndexOf('/')+1;
            topic = topic.Substring(index);
            ConvertToEvent(topic, json);
            return Task.CompletedTask;
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            Aci.Unity.Logging.AciLog.Log("Network", "Connection to mqtt-server lost.");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                Aci.Unity.Logging.AciLog.Log("Network", "Trying to reconnect to mqtt-server...");
                await m_Client.ConnectAsync(m_ClientOptions, CancellationToken.None);
            }
            catch
            {
                Aci.Unity.Logging.AciLog.Log("Network", "Reconnect failed. Trying again in 5 sec.");
            }
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            Aci.Unity.Logging.AciLog.Log("Network", "Connected to mqtt-server.");

            // Subscribe to a topic
            await m_Client.SubscribeAsync(new TopicFilterBuilder().WithTopic(MQTTComponents.Table + "/#").Build());
            await m_Client.SubscribeAsync(new TopicFilterBuilder().WithTopic(MQTTComponents.Mouse + "/#").Build());
            await m_Client.SubscribeAsync(new TopicFilterBuilder().WithTopic(MQTTComponents.Control + "/#").Build());
            string json = JsonUtility.ToJson(new Status() { state = (uint)MQTTStatus.Ready, error = "" });
            SendMessage(MQTTComponents.Control, MQTTTopics.Status, json);

            Aci.Unity.Logging.AciLog.Log("Network", $"Subscribed to topic {MQTTComponents.Control}/#");
        }

        public async void SendQCMessage(int part)
        {
            if (!(m_Client?.IsConnected ?? false))
                return;
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic("QC_REQUEST_MSG")
                .WithPayload(part.ToString())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await m_Client.PublishAsync(message);
        }

        public async void SendMessage(string component, string topic, string payload)
        {
            if (!(m_Client?.IsConnected ?? false))
                return;

            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(component + "/" + topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            Aci.Unity.Logging.AciLog.Log("Network", $"Published Message: {payload}");

            await m_Client.PublishAsync(message);
        }

        /// <summary>
        /// Returns a new unique request id.
        /// </summary>
        /// <remarks>
        /// The first 3 bits are reserved for preventing duplicate ids between different system components.
        /// This makes it possible to use up to 8 different system components.
        /// The max amount of unique identifiers per system is 536870912.
        /// The value will reset if reaching the max value.
        /// The amount should prevent having duplicate values in the system anywhere.
        /// </remarks>
        /// <returns>The new unsigned int request id.</returns>
        public uint GetNewRequestId()
        {
            m_RequestIdCounter = (m_RequestIdCounter+1) & m_SubtractionMask;
            return m_ComponentMask | m_RequestIdCounter;
        }

        private void ConvertToEvent(string topic, string json)
        {
            Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            switch (topic)
            {
                case "Status":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Status>(json)));
                    break;
                case "assembly_order_req":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_order_req>(json)));
                    break;
                case "assembly_order_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_order_ack>(json)));
                    break;
                case "assembly_order_res":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_order_res>(json)));
                    break;
                case "insert_board_req":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Insert_board_req>(json)));
                    break;
                case "insert_board_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Insert_board_ack>(json)));
                    break;
                case "insert_board_res":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Insert_board_res>(json)));
                    break;
                case "detect_reference_mark_req":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Detect_reference_mark_req>(json)));
                    break;
                case "detect_reference_mark_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Detect_reference_mark_ack>(json)));
                    break;
                case "detect_reference_mark_res":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Detect_reference_mark_res>(json)));
                    break;
                case "heading_angle_psi_req":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Heading_angle_psi_req>(json)));
                    break;
                case "heading_angle_psi_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Heading_angle_psi_ack>(json)));
                    break;
                case "heading_angle_psi_res":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Heading_angle_psi_res>(json)));
                    break;
                case "assembly_step_req":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_step_req>(json)));
                    break;
                case "assembly_step_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_step_ack>(json)));
                    break;
                case "assembly_step_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_step_res>(json)));
                    break;
                case "detect_part_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_part_req>(json)));
                    break;
                case "detect_part_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_part_ack>(json)));
                    break;
                case "detect_part_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_part_res>(json)));
                    break;
                case "detect_assembly_position_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_assembly_position_req>(json)));
                    break;
                case "detect_assembly_position_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_assembly_position_ack>(json)));
                    break;
                case "detect_assembly_position_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_assembly_position_res>(json)));
                    break;
                case "assembly_part_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_part_req>(json)));
                    break;
                case "assembly_part_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_part_ack>(json)));
                    break;
                case "assembly_part_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_part_res>(json)));
                    break;
                case "detect_gripper_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_gripper_req>(json)));
                    break;
                case "detect_gripper_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_gripper_ack>(json)));
                    break;
                case "detect_gripper_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Detect_gripper_res>(json)));
                    break;
                case "gripper_offset_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_offset_req>(json)));
                    break;
                case "gripper_offset_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_offset_ack>(json)));
                    break;
                case "gripper_offset_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_offset_res>(json)));
                    break;
                case "in_position_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<In_position_req>(json)));
                    break;
                case "in_position_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<In_position_ack>(json)));
                    break;
                case "in_position_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<In_position_res>(json)));
                    break;
                case "assembly_done_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_done_req>(json)));
                    break;
                case "assembly_done_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_done_ack>(json)));
                    break;
                case "assembly_done_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_done_res>(json)));
                    break;
                case "inspect_single_part_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_single_part_req>(json)));
                    break;
                case "inspect_single_part_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_single_part_ack>(json)));
                    break;
                case "inspect_single_part_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_single_part_res>(json)));
                    break;
                case "assembly_end_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_end_req>(json)));
                    break;
                case "assembly_end_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_end_ack>(json)));
                    break;
                case "assembly_end_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Assembly_end_res>(json)));
                    break;
                case "inspect_assembly_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_assembly_req>(json)));
                    break;
                case "inspect_assembly_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_assembly_ack>(json)));
                    break;
                case "inspect_assembly_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspect_assembly_res>(json)));
                    break;
                case "inspection_loop_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspection_loop_req>(json)));
                    break;
                case "inspection_loop_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspection_loop_ack>(json)));
                    break;
                case "inspection_loop_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Inspection_loop_res>(json)));
                    break;
                case "pressured_air_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Pressured_air_req>(json)));
                    break;
                case "pressured_air_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Pressured_air_ack>(json)));
                    break;
                case "pressured_air_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Pressured_air_res>(json)));
                    break;
                case "reset_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Reset_req>(json)));
                    break;
                case "reset_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Reset_ack>(json)));
                    break;
                case "reset_res":
                    UnityMainThreadDispatcher.Instance().Enqueue( () => m_EventManager.Invoke(JsonUtility.FromJson<Reset_res>(json)));
                    break;
                case "guide_gripper_drop_off_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_drop_off_req>(json)));
                    break;
                case "guide_gripper_drop_off_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_drop_off_ack>(json)));
                    break;
                case "guide_gripper_drop_off_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_drop_off_res>(json)));
                    break;
                case "guide_gripper_pick_up_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_pick_up_req>(json)));
                    break;
                case "guide_gripper_pick_up_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_pick_up_ack>(json)));
                    break;
                case "guide_gripper_pick_up_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Guide_gripper_pick_up_res>(json)));
                    break;
                case "gripper_ready_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_ready_req>(json)));
                    break;
                case "gripper_ready_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_ready_ack>(json)));
                    break;
                case "gripper_ready_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Gripper_ready_res>(json)));
                    break;
                case "part_picked_req":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Part_picked_req>(json)));
                    break;
                case "part_picked_ack":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Part_picked_ack>(json)));
                    break;
                case "part_picked_res":
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(JsonUtility.FromJson<Part_picked_res>(json)));
                    break;
                default:
                    AciLog.Log("MQTTConnector", $"Unknown message type {topic} with payload: {json}");
                    break;
            }
        }
    }
}
