// <copyright file=ChatTextInput.cs/>
// <copyright>
//   Copyright © 2018, UID
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
//   Jannik Lassahn, Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <creation date>8/7/2018 14:27</creation date>
// <last update>8/14/2018 14:24</last update>
using System;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using Zenject;

using Aci.Unity.Bot;

namespace BotConnector.Unity.UI
{
    public class ChatTextInput : MonoBehaviour, INotifyChatInput
    {
        //[Inject]
        //private BotConnect botConnector;

        public InputField Input;
        public Button SendButton;

        public event Action<string> ChatInputReceived;

        private void Start()
        {
            Assert.IsNotNull(Input, "Expected an input field for ChatTextInput component");
            Assert.IsNotNull(SendButton, "Expected a button for ChatTextInput component");

            Input.onEndEdit.AddListener(input =>
            {
                //botConnector.SendMessageAsync(input,true);
                Input.text = string.Empty;
            });
            Input.onValueChanged.AddListener(input => SendButton.interactable = !string.IsNullOrEmpty(input));

            SendButton.onClick.AddListener(() => ChatInputReceived?.Invoke(Input.text));
            SendButton.interactable = false;
        }
    }

}
