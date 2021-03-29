using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UI
{
    public class ImageColorReplacement : MonoBehaviour, IMaterialModifier
    {
        [SerializeField]
        private Color m_ColorA;
        [SerializeField]
        private Color m_ColorB;

        private int m_ColorAHash;
        private int m_ColorBHash;
        private Material m_Material;

        public Color colorA
        {
            get => m_ColorA;
            set
            {
                if (m_ColorA == value)
                    return;

                m_ColorA = value;
            }
        }

        public Color colorB
        {
            get => m_ColorB;
            set
            {
                if (m_ColorB == value)
                    return;

                m_ColorB = value;
            }
        }

        private void Awake()
        {
            m_ColorAHash = Shader.PropertyToID("_ColorA");
            m_ColorBHash = Shader.PropertyToID("_ColorB");
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!Application.isPlaying)
                return baseMaterial;

            if (m_Material == null)
                m_Material = Instantiate(baseMaterial);

            m_Material.SetColor(m_ColorAHash, colorA);
            m_Material.SetColor(m_ColorBHash, colorB);
            return m_Material;
        }
    }
}