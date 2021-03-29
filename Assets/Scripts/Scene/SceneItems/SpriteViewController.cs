// <copyright file=SceneSprite.cs/>
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
// <date>07/12/2018 05:59</date>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aci.Unity.Services;
using Aci.Unity.Util;
using UnityEngine;

namespace Aci.Unity.Scene.SceneItems
{
    /// <summary>
    ///     SceneRect can be used to render unity asset based textures.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteViewController : MonoBehaviour, IPayloadViewController, IScalable
    {
        private ITimeProvider                           m_TimeProvider;
        private IConfigProvider                         m_ConfigProvider;
        private ICachedResourceProvider<Sprite, string> m_ImageService;
        private BoxCollider                             m_Collider;
        private SpriteRenderer                          m_Image;
        private Sprite                                  m_Sprite;

        private PayloadType m_PayloadType;
        public  PayloadType payloadType => m_PayloadType;

        private string m_Payload;
        public  string payload => m_Payload;

        private float m_Delay;

        private ItemMode m_Mode;

        private CancellationTokenSource _source = new CancellationTokenSource();

        [ConfigValue("workflowDirectory")]
        private string workflowDirectory { get; set; } = "";

        public float delay
        { 
            get => m_Delay;
            set
            {
                if (m_Delay == value)
                    return;
                m_Delay = value;
            }
        }

        private Vector3 m_ScaleFactor;
        public Vector3 size
        {
            get => m_ScaleFactor;
            set
            {
                m_ScaleFactor = value;
                EvaluateScale();
            }

        }

        [Zenject.Inject]
        private void Construct(ICachedResourceProvider<Sprite, string> imageService,
                               ItemMode mode,
                               IConfigProvider configProvider,
                               [Zenject.InjectOptional] ITimeProvider timeProvider)
        {
            m_TimeProvider = timeProvider;
            m_Mode = mode;
            m_ImageService = imageService;
            m_ConfigProvider = configProvider;
        }

        private void Awake()
        {
            m_ConfigProvider.RegisterClient(this);
            m_Image = GetComponent<SpriteRenderer>();
            if (m_Sprite == null)
                m_Sprite = m_Image.sprite;
            m_Collider = GetComponent<BoxCollider>();
        }

        private void OnDestroy()
        {
            m_ConfigProvider.UnregisterClient(this);
        }

        private async void OnEnable()
        {
            CancellationToken token = _source.Token;
            m_Image.enabled = false;
            if (m_Mode == ItemMode.Workflow)
            {
                float time = Mathf.Max(m_Delay - (float)m_TimeProvider.elapsed.TotalSeconds, 0);
                await Task.Delay(Mathf.FloorToInt(time * 1000), token);
                if (token.IsCancellationRequested)
                    return;
            }
            m_Image.enabled = true;
        }

        public void SetPayload(PayloadType type, string payload, float delay)
        {
            if (!(type == PayloadType.Primitive || type == PayloadType.Image))
                return;
            m_PayloadType = type;
            m_Payload = payload;
            m_Delay = delay;
            if(type != PayloadType.Primitive)
                LoadSprite();
        }

        private async void LoadSprite()
        {
            try
            {
                CancellationToken token = _source.Token;
                m_Sprite = await m_ImageService.Get("file://" + Path.Combine(workflowDirectory, payload));
                if (token.IsCancellationRequested)
                    return;
                m_Image.sprite = m_Sprite;
                EvaluateScale();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void EvaluateScale()
        {
            // base size is always measured by the larger axis
            float aspectRatio = m_Sprite.rect.height / m_Sprite.rect.width;
            Vector2 size = new Vector2((aspectRatio < 1) ? 100 : 100 * m_Sprite.rect.width / m_Sprite.rect.height,
                                       (aspectRatio < 1) ? 100 * aspectRatio : 100);
            if (m_Image.drawMode == SpriteDrawMode.Simple)
            {
                size.x = m_Image.sprite.rect.width;
                size.y = m_Image.sprite.rect.height;
                m_Collider.size = new Vector2(size.x, size.y);
                size.x = (size.x > 0) ? 100 / size.x : 1;
                size.y = (size.y > 0) ? 100 / size.y : 1;
                transform.localScale = new Vector3(m_ScaleFactor.x * size.x, m_ScaleFactor.y * size.y, m_ScaleFactor.z);
            }
            else
            {
                transform.localScale = Vector3.one;
                size = size * m_ScaleFactor;
                m_Image.size = size;
                m_Collider.size = new Vector3(size.x, size.y, m_ScaleFactor.z);

                // get minimum size of rect
                Vector2 spriteMin = new Vector2(m_Sprite.border[0] + m_Sprite.border[2],
                                                m_Sprite.border[1] + m_Sprite.border[3]);

                if (size.x < spriteMin.x || size.y < spriteMin.y)
                {
                    Vector2 scalingOffset = spriteMin / size;
                    float scaleModifier = Mathf.Max(scalingOffset.x, scalingOffset.y);
                    m_Image.size = size * scaleModifier;
                    m_Collider.size = m_Image.size;
                    Vector3 localScale = Vector3.one / scaleModifier;
                    localScale.z = 1;
                    transform.localScale = localScale;
                }
            }
        }
    }
}