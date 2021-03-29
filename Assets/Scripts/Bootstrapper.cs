using UnityEngine;
using UnityEngine.SceneManagement;
using Aci.Unity.Logging;

namespace Aci.Unity
{
    public class Bootstrapper : MonoBehaviour
    {
        public string[] scenesToLoad;

        void Awake()
        {
            LoadScene();

            if (!Debug.isDebugBuild)
                Debug.unityLogger.logEnabled = false;
        }

        private void LoadScene()
        {
            foreach (string sceneToLoad in scenesToLoad)
            {
                bool sceneLoaded = false;
                for (int i = 0; i < SceneManager.sceneCount; ++i)
                {
                    if (SceneManager.GetSceneAt(i).name == sceneToLoad)
                    {
                        sceneLoaded = true;
                        break;
                    }
                }
                if (sceneLoaded)
                    continue;
                AciLog.Log("Bootstrapper", $"Loading scene {sceneToLoad}...");
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            }
        }
    }
}