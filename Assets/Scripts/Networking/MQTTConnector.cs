using Aci.Unity.Events;
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
    public class MQTTConnector : MonoBehaviour
                               , IMqttClientConnectedHandler
                               , IMqttClientDisconnectedHandler
                               , IMqttApplicationMessageReceivedHandler
    {
        private IAciEventManager m_EventManager;
        private IMqttClientFactory m_Factory;
        private IMqttClient m_Client;
        private IMqttClientOptions m_ClientOptions;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager)
        {
            m_EventManager = eventManager;
        }

        private async void Start()
        {
            m_Factory = new MqttFactory();
            m_Client = m_Factory.CreateMqttClient();

            m_ClientOptions = new MqttClientOptionsBuilder()
             .WithClientId("incluMOVE-GUI")
             .WithTcpServer("localhost", 1883)
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
            if(bool.TryParse(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload), out bool result))
            {
                if (result)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => m_EventManager.Invoke(new CVTriggerArgs() { okay = result }));
                }
            }
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
            await m_Client.SubscribeAsync(new TopicFilterBuilder().WithTopic("QC_RESPONSE_MSG").Build());

            Aci.Unity.Logging.AciLog.Log("Network", "Subscribed to QC_RESPONSE_MSG.");
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
    }
}
