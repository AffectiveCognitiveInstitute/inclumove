using RSG;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Microsoft.Bot.Connector.DirectLine;

namespace BotConnector.Unity.UI
{
    public class RenderContext : IRenderContext
    {

        #region Fields

        private IRenderSettings settings;
        private GameObject rootPrefab;
        private MonoBehaviour dispatcher;
        private Func<CardAction, bool> cardActionHandler;

        #endregion

        #region Properties

        public Theme Theme { get { return settings.Theme; } }

        public IPromise Status { get; }

        #endregion

        #region Constructor

        public RenderContext(IRenderSettings settings, IPromise status, GameObject rootPrefab, Func<CardAction, bool> handler)
        {
            this.settings = settings;
            this.rootPrefab = rootPrefab;
            cardActionHandler = handler;
            Status = status;

            if (settings is MonoBehaviour)
                dispatcher = settings as MonoBehaviour;
        }

        #endregion

        public virtual IPromise<Texture2D> GetTextureFromUri(string uri)
        {
            var promise = new Promise<Texture2D>();
            dispatcher.StartCoroutine(GetTextureFromUri(promise, new Uri(uri)));
            return promise;
        }

        public virtual IPromise<Sprite> GetSpriteFromUri(string uri)
        {
            var promise = new Promise<Sprite>();
            GetTextureFromUri(uri)
                .Then(texture => GetSpriteFromTexture(promise, texture));
            return promise;
        }

        private IEnumerator GetTextureFromUri(Promise<Texture2D> promise, Uri uri)
        {
            var request = UnityWebRequestTexture.GetTexture(uri.ToString());
            yield return request.SendWebRequest();

            if (!(request.isHttpError || request.isNetworkError))
            {
                try
                {
                    promise.Resolve(DownloadHandlerTexture.GetContent(request));
                }
                catch (Exception e)
                {
                    promise.Reject(e);
                }
            }
        }

        private void GetSpriteFromTexture(Promise<Sprite> promise, Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            promise.Resolve(sprite);
        }

        public virtual bool InvokeCardAction(CardAction action)
        {
            if (action.Type == null)
                return false;
            return cardActionHandler(action);
        }

        public virtual GameObject RenderAttachment(Attachment attachment)
        {
            AttachmentHandler handler;
            settings.Theme.Attachments.TryGetValue(attachment.ContentType, out handler);

            if(handler == null)
            {
                Debug.LogError($"[BOT] No handler for attachment with type '{attachment.ContentType}'");
                return null;
            }

            var template = GameObject.Instantiate(handler.Template);
            var renderer = template.GetComponentInChildren<IRenderer>();

            if (renderer == null)
            {
                Debug.LogError($"[BOT] Missing renderer in template of attachment handler with type {attachment.ContentType}.");
            }
            else
            {
                renderer.Render(attachment, this);
            }

            return template;
        }

        public virtual void RenderToTarget<T>(GameObject target, T content)
        {
            var rendererType = settings.ResolveCustomRenderer(content);
            if(rendererType != null)
            {
                var renderer = (IRenderer)target.GetComponentInChildren(rendererType);
                if (renderer == null && settings.AddMissingRenderer)
                    renderer = (IRenderer)target.AddComponent(rendererType);
                renderer?.Render(content, this);
            }
        }

        public virtual GameObject Render<T>(T content)
        {
            GameObject render = null;

            if (content is Activity)
            {
                render = GameObject.Instantiate(rootPrefab);
                render.GetComponent<ActivityRenderer>().Render(content, this);
            }

            else if(content is Attachment)
            {
                render = RenderAttachment(content as Attachment);
            }

            return render;
        }
    }
}

