using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BotConnector.Unity
{
    [CustomEditor(typeof(Bot))]
    [CanEditMultipleObjects]
    [Serializable]
    public class BotEditor : Editor
    {
        #region Enums

        enum Endpoint
        {
            Default,
            Custom
        }

        #endregion

        #region Private fields

        private SerializedProperty secret, connectivity, pollingInterval, autoStart, onlyBotMessages, domain, userId, userName, handle, messageDelay;
        private SerializedProperty onMessageReceived, onConnectionStatusChanged;

        private string domainTemp;

        [SerializeField]
        private static Endpoint endpoint;
        private List<string> errors = new List<string>();

        #endregion

        #region Unity methods

        private void OnEnable()
        {
            secret = serializedObject.FindProperty("secret");
            connectivity = serializedObject.FindProperty("connectivity");
            pollingInterval = serializedObject.FindProperty("pollingInterval");
            autoStart = serializedObject.FindProperty("autoStart");
            onlyBotMessages = serializedObject.FindProperty("onlyBotMessages");
            domain = serializedObject.FindProperty("domain");
            userId = serializedObject.FindProperty("userId");
            userName = serializedObject.FindProperty("userName");
            handle = serializedObject.FindProperty("handle");
            messageDelay = serializedObject.FindProperty("messageDelay");

            onMessageReceived = serializedObject.FindProperty("onMessageReceived");
            onConnectionStatusChanged = serializedObject.FindProperty("onConnectionStatusChanged");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            HeaderWithTitle("Connection");

            EditorGUILayout.PropertyField(secret);
            EditorGUILayout.PropertyField(handle);

            DrawConnectivityProperties();
            DrawEndpointProperty();

            HeaderWithTitle("User");

            EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(onlyBotMessages);

            DrawChannelAccount();

            HeaderWithTitle("Advanced");

            EditorGUILayout.PropertyField(messageDelay);

            HeaderWithTitle("Events");

            EditorGUILayout.PropertyField(onMessageReceived);
            EditorGUILayout.PropertyField(onConnectionStatusChanged);

            DrawHelpBoxes();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private methods

        private void HeaderWithTitle(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private void DrawChannelAccount()
        {
            EditorGUILayout.PrefixLabel(new GUIContent("Account"));

            Indent(() =>
            {
                EditorGUILayout.PropertyField(userId, new GUIContent("User ID"));
                EditorGUILayout.PropertyField(userName, new GUIContent("User Name"));
            });
        }

        private void DrawConnectivityProperties()
        {
            EditorGUILayout.PropertyField(connectivity);

            if((Connectivity)connectivity.intValue == Connectivity.PollingOverHTTP)
            {
                Indent(() => EditorGUILayout.PropertyField(pollingInterval, new GUIContent("Polling Interval")));
            }
        }

        private void DrawEndpointProperty()
        {
            EditorGUI.BeginChangeCheck();

            endpoint = (Endpoint)EditorPrefs.GetInt("Endpoint");
            endpoint = (Endpoint)EditorGUILayout.EnumPopup("Endpoint", endpoint);

            if(endpoint == Endpoint.Custom)
            {
                Indent(() => domainTemp = EditorGUILayout.TextField("Domain", domain.stringValue));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("Endpoint", (int)endpoint);
                domain.stringValue = endpoint == Endpoint.Default ? Bot.DirectLineEndpoint : domainTemp;
            }
        }

        private void DrawHelpBoxes()
        {
            errors.Clear();

            if (endpoint == Endpoint.Custom && string.IsNullOrEmpty(domain.stringValue))
                errors.Add("You need to set a valid domain as custom endpoint.");

            if (string.IsNullOrWhiteSpace(secret.stringValue))
                errors.Add("You need to set a secret to access a bot.");

            if (string.IsNullOrEmpty(handle.stringValue))
                errors.Add("You need to set the bot handle to know which activities come from the bot.");

            if(errors.Count > 0)
            {
                HeaderWithTitle("Warnings");
                errors.ForEach(x => EditorGUILayout.HelpBox(x, MessageType.Warning));
            }
        }

        private void Indent(Action action)
        {
            EditorGUI.indentLevel++;
            action();
            EditorGUI.indentLevel--;
        }

        #endregion
    }

}

