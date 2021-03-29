using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aci.Unity.Events;
using UnityEngine;
using WebSocketSharp;
using Zenject;

namespace Aci.Unity.Util
{
    public class UsbDetectorService : ITickable
    {
        private IAciEventManager m_EventManager;
        private List<string> m_ConnectedDrives = new List<string>();
        private string m_SelectedDrive = null;
        private string m_SelectedDirectory = null;
        private float m_LastTime = 0;
        private float m_QueryRate = 5f;

        [ConfigValue("UseUsbProfile")]
        public bool m_UseUsbProfile { get; set; } = true;

        public string currentDrive => m_SelectedDrive;
        public string currentDirectory => m_SelectedDirectory;

        public bool active { get; set; } = false;

        public UsbDetectorService(IAciEventManager eventManager, IConfigProvider configProvider)
        {
            m_EventManager = eventManager;
            configProvider.RegisterClient(this);
            configProvider.ClientDirty(this);
        }
        
        private void QueryDeviceChange()
        {
            if (!m_UseUsbProfile)
            {
                if (!m_SelectedDrive.IsNullOrEmpty())
                    return;
                m_SelectedDrive = Application.dataPath + "/../";
                m_SelectedDirectory = Application.dataPath + "/../";
                m_EventManager.Invoke(new USBDevice() { drive = m_SelectedDrive });
                return;
            }
            // query for changes in current drives
            List<string> drives = DriveInfo.GetDrives().Where(info => info.DriveType == DriveType.Removable).Select(info => info.VolumeLabel).ToList();
            IEnumerable<string> changes = m_ConnectedDrives.Concat(drives).Distinct();
            m_ConnectedDrives = drives;
            // if we currently do have selected a drive, check if disconnected
            if (changes.Contains(m_SelectedDrive))
            {
                // drive still connected, no change
                if (drives.Contains(m_SelectedDrive))
                    return;
                // drive disconnected
                m_SelectedDrive = null;
                m_EventManager.Invoke(new USBDevice() { drive = null });
                Aci.Unity.Logging.AciLog.Log("Log", "USBDrive disconnected.");
                return;
            }
            // if we currently do not have a selected drive check for connection
            foreach (string drive in changes)
            {
                DriveInfo info = new DriveInfo(drive);
                m_SelectedDirectory = info.RootDirectory.ToString();
                string filePath = m_SelectedDirectory + "user.prf";
                m_SelectedDrive = drive;
                m_EventManager.Invoke(new USBDevice() { drive = m_SelectedDrive });
                Aci.Unity.Logging.AciLog.Log("Log", "USBDrive connected.");
            }
        }

        public void Tick()
        {
            if (!active)
                return;
            if (Time.time - m_LastTime < m_QueryRate)
                return;
            m_LastTime = Time.time;
            QueryDeviceChange();
        }
    }
}