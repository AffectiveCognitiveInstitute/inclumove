using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class GameObjectVisibilityController : MonoBehaviour
    {
        public bool isActive
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
    }
}