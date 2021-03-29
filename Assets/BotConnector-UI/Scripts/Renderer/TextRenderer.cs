using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    public class TextRenderer : RendererBase<string>
    {
        public Text Text;

        private void Awake()
        {
            Text = Text ?? GetComponentInChildren<Text>();
        }

        public override void Render(string content, IRenderContext context)
        {
            Text.text = content;
        }
    }
}


