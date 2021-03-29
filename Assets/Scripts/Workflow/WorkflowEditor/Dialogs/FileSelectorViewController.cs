using Aci.Unity.UI.Dialog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public struct FileSelectedEventArgs
    {
        public string filePath;
    }

    public delegate void FileSelectedDelegate(object sender, FileSelectedEventArgs eventArgs);

    [RequireComponent(typeof(DialogComponent))]
    public class FileSelectorViewController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<FileSelectedDelegate, IEnumerable<string>, IEnumerable<string>, FileSelectorViewController> { }

        [SerializeField]
        private FileSpriteRegistry m_FileSpriteRegistry;

        [SerializeField]
        public GameObject m_FilePrefab;

        [SerializeField]
        public Transform m_Container;

        private List<string> m_SearchPaths = new List<string>();
        private List<string> m_Filters = new List<string>();
        private List<FileViewController> m_Files = new List<FileViewController>();
        private List<string> m_SearchResults = new List<string>();
        private DialogComponent m_DialogComponent;

        public event FileSelectedDelegate fileSelected;

        [Zenject.Inject]
        public void Construct(FileSelectedDelegate fileSelected, IEnumerable<string> searchPaths, IEnumerable<string> filters)
        {
            this.fileSelected += fileSelected;
            foreach (string path in searchPaths)
                AddSearchPath(path, false);

            foreach (string filter in filters)
                AddFilter(filter, false);
        }

        private void Awake()
        {
            m_DialogComponent = GetComponent<DialogComponent>();
        }

        private void OnEnable()
        {
            m_DialogComponent.dismissed += OnDialogDismissed;
            SetDirty();
        }

        private void OnDisable()
        {
            m_DialogComponent.dismissed -= OnDialogDismissed;
        }

        private void OnDialogDismissed(IDialog dialog)
        {
            Destroy(gameObject);
        }


        public void AddSearchPath(string path, bool setDirty = true)
        {
            if (!Directory.Exists(path))
                return;

            m_SearchPaths.Add(path);
            if(setDirty)
                SetDirty();
        }

        public void RemoveSearchPath(string path, bool setDirty = true)
        {
            m_SearchPaths.Remove(path);
            if(setDirty)
                SetDirty();
        }

        public void AddFilter(string filter, bool setDirty = true)
        {
            if (m_Filters.Contains(filter))
                return;

            m_Filters.Add(filter);
            if(setDirty)
                SetDirty();
        }

        public void RemoveFilter(string filter, bool setDirty = true)
        {
            if (m_Filters.Remove(filter))
                if(setDirty)
                    SetDirty();
        }

        private void SetDirty()
        {
            for (int i = 0; i < m_Files.Count; i++)
            {
                m_Files[i].fileSelected -= OnFileSelected;
                Destroy(m_Files[i].gameObject);
            }

            m_SearchResults.Clear();

            for(int i = 0; i < m_SearchPaths.Count; i++)
            {
                if(m_Filters.Count == 0)
                {
                    string[] fileEntries = Directory.GetFiles(m_SearchPaths[i]);
                    for (int j = 0; j < fileEntries.Length; j++)
                    {
                        string path = fileEntries[j].Replace("\\", "/");
                        m_SearchResults.Add(path);
                    }
                }
                else
                {
                    for(int j = 0; j < m_Filters.Count; j++)
                    {
                        string[] fileEntries = Directory.GetFiles(m_SearchPaths[i], m_Filters[j], SearchOption.AllDirectories); // TODO: SearchOption should be configurable
                        for(int k = 0; k < fileEntries.Length; k++)
                        {
                            string path = fileEntries[k].Replace("\\", "/");
                            m_SearchResults.Add(path);
                        }
                    }
                }
            }

            m_SearchResults = m_SearchResults.OrderBy(path => Path.GetExtension(path)).ToList();
            for(int i = 0; i < m_SearchResults.Count; i++)
            {
                GameObject go = Instantiate(m_FilePrefab);
                FileViewController vc = go.GetComponent<FileViewController>();
                go.transform.SetParent(m_Container, false);
                vc.Initialize(m_SearchResults[i], m_FileSpriteRegistry.GetSprite(Path.GetExtension(m_SearchResults[i])));
                vc.fileSelected += OnFileSelected;
            }
        }

        private void OnFileSelected(object sender, FileSelectedEventArgs eventArgs)
        {
            m_DialogComponent.Dismiss();
            fileSelected?.Invoke(this, eventArgs);
        }

        public static implicit operator DialogComponent(FileSelectorViewController vc)
        {
            return vc.m_DialogComponent ?? vc.GetComponent<DialogComponent>();
        }
    }
}