using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class SteamManager
    {
        private readonly uint _appId = 2161040;
        
        public readonly bool Initialized;
        public Action OnInitialized = () => { };

        public SteamManager()
        {
            try
            {
                if (SteamClient.RestartAppIfNecessary(_appId))
                {
                    Application.Quit();
                    return;
                }

                SteamClient.Init(_appId);

                Initialized = true;
                OnInitialized.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load Steam API\n" + e);
                Application.Quit();
            }
        }

        public void Update()
        {
            if (!Initialized) return;
            SteamClient.RunCallbacks();
        }

        public void OnDestroy()
        {
            if (!Initialized) return;
            SteamClient.Shutdown();
        }

        public SteamId GetMySteamID()
        {
            return SteamClient.SteamId;
        }
    }
}