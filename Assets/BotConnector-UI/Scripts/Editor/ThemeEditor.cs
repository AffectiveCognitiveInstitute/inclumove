using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [CustomEditor(typeof(Theme))]
    public class ThemeEditor : Editor
    {
        private SerializedProperty bot, user, attachments, button, suggestedAction, carousel;
        private ReorderableList attachmentsList;
        private GenericMenu menu;
        private HashSet<string> currentHandlers;
        private bool showHandlerWarning = false;
        private GUIContent removeItemIcon;
        private GUIStyle removeItemStyle;


        private void OnEnable()
        {
            bot = serializedObject.FindProperty(nameof(Theme.BotActivity));
            user = serializedObject.FindProperty(nameof(Theme.UserActivity));
            button = serializedObject.FindProperty(nameof(Theme.Button));
            carousel = serializedObject.FindProperty(nameof(Theme.Carousel));
            suggestedAction = serializedObject.FindProperty(nameof(Theme.SuggestedAction));

            attachments = serializedObject.FindProperty("attachments");
            SetupAttachmentsList();

            removeItemIcon = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove selection from list");
        }

        private void SetupAttachmentsList()
        {
            attachmentsList = Helper.GetListWithFoldout(serializedObject, attachments, false, true, true, false);
            attachmentsList.elementHeightCallback += index => attachments.isExpanded ? Helper.SingleLineHeight : 0;
            attachmentsList.drawElementCallback += (rect, index, active, focused) =>
            {
                if (!attachments.isExpanded)
                    return;

                rect.height = 16;
                rect.width = rect.width - 22;
                rect.y += 2;

                EditorGUI.PropertyField(rect, attachmentsList.serializedProperty.GetArrayElementAtIndex(index));

                rect.x = rect.width + 24;
                rect.width = 16;

                if (GUI.Button(rect, removeItemIcon, removeItemStyle))
                    attachmentsList.serializedProperty.DeleteArrayElementAtIndex(index);
            };
            attachmentsList.onAddDropdownCallback = (rect, list) =>
            {
                //menu = new GenericMenu();
                //if (!currentHandlers.Contains(AttachmentConverter.AnimationCardType))
                //    menu.AddItem(new GUIContent("Animation"), false, OnAddCard, AttachmentConverter.AnimationCardType);
                //if (!currentHandlers.Contains(AttachmentConverter.AudioCardType))
                //    menu.AddItem(new GUIContent("Audio"), false, OnAddCard, AttachmentConverter.AudioCardType);
                //if (!currentHandlers.Contains(AttachmentConverter.HeroCardType))
                //    menu.AddItem(new GUIContent("Hero"), false, OnAddCard, AttachmentConverter.HeroCardType);
                //if (!currentHandlers.Contains(AttachmentConverter.ReceiptCardType))
                //    menu.AddItem(new GUIContent("Receipt"), false, OnAddCard, AttachmentConverter.ReceiptCardType);
                //if (!currentHandlers.Contains(AttachmentConverter.ThumbnailCardType))
                //    menu.AddItem(new GUIContent("Thumbnail"), false, OnAddCard, AttachmentConverter.ThumbnailCardType);
                //if (!currentHandlers.Contains(AttachmentConverter.VideoCardType))
                //    menu.AddItem(new GUIContent("Video"), false, OnAddCard, AttachmentConverter.VideoCardType);
                //menu.AddSeparator(string.Empty);
                //menu.AddItem(new GUIContent("Custom"), false, OnAddCard, string.Empty);
                //menu.ShowAsContext();
            };
            attachmentsList.onRemoveCallback += (ReorderableList l) =>
            {
                currentHandlers.Remove(l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("ContentType").stringValue);

                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                if (l.index >= l.serializedProperty.arraySize - 1)
                {
                    l.index = l.serializedProperty.arraySize - 1;
                }
            };
        }

        private void UpdateHandlers()
        {
            currentHandlers = new HashSet<string>();
            showHandlerWarning = false;

            for (int i = 0; i < attachmentsList.count; i++)
            {
                string value = attachmentsList.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("ContentType").stringValue;
                if (currentHandlers.Contains(value))
                {
                    showHandlerWarning = true;
                    continue;
                }
                currentHandlers.Add(value);
            }
        }

        private void OnAddCard(object contentType)
        {
            currentHandlers.Add((string)contentType);

            var index = attachmentsList.serializedProperty.arraySize;
            attachmentsList.serializedProperty.arraySize++;
            attachmentsList.index = index;
            var element = attachmentsList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("ContentType");
            element.stringValue = (string)contentType;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            UpdateHandlers();

            serializedObject.Update();

            EditorGUILayout.PropertyField(bot);
            EditorGUILayout.PropertyField(user);
            EditorGUILayout.PropertyField(button);
            EditorGUILayout.PropertyField(suggestedAction);
            EditorGUILayout.PropertyField(carousel);

            GUILayout.Space(10);

            if(removeItemStyle == null)
                removeItemStyle = new GUIStyle(GUI.skin.GetStyle("RL FooterButton"));

            attachmentsList.DoLayoutList();

            if (showHandlerWarning)
                EditorGUILayout.HelpBox("You cannot have multiple handlers for a single content type.", MessageType.Error);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

