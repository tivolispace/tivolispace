using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tivoli.Scripts.Managers
{
    public class VRManager : Manager
    {
#if UNITY_EDITOR
        private readonly bool _startInVr =
            EditorPrefs.GetBool(TivoliEditorPrefs.PlayInVR, TivoliDefaultEditorPrefs.PlayInVR);
#else
        private readonly bool _startInVr = Environment.GetEnvironmentVariable("DISABLE_VR") == null;
#endif

        public override Task Init()
        {
            if (!_startInVr) return Task.CompletedTask;

            var manager = XRGeneralSettings.Instance.Manager;
            manager.InitializeLoaderSync();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed");
            }
            else
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }

            return Task.CompletedTask;
        }

        public override void OnDestroy()
        {
            if (!_startInVr) return;

            var manager = XRGeneralSettings.Instance.Manager;
            if (!manager.isInitializationComplete) return;

            Debug.Log("Stopping XR...");
            manager.StopSubsystems();
            manager.DeinitializeLoader();
            Debug.Log("XR stopped completely.");
        }
    }
}