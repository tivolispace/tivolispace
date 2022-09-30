using System;
using Mirror.FizzySteam;
using Tivoli.Scripts.Networking;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tivoli.Scripts.Managers
{
    public class ConnectionManager
    {
        private TivoliNetworkManager _networkManager;
        private FizzyFacepunch _fizzyFacepunch;
        
        // TivoliNetworkManager sets this value
        public bool Hosting;
        public bool InWorld;

        public ConnectionManager()
        {
            Init();
        }


        private async void Init()
        {
            var steamManager = DependencyManager.Instance.steamManager;
            await steamManager.WhenInitialized();

            _networkManager = Object.FindObjectOfType<TivoliNetworkManager>();
            _fizzyFacepunch = _networkManager.GetComponent<FizzyFacepunch>();

            _fizzyFacepunch.SteamAppID = SteamManager.AppId;
            _fizzyFacepunch.SteamUserID = steamManager.GetMySteamID();
            _fizzyFacepunch.Init();
        }

        public void StartHosting()
        {
            Debug.Log("Hosting started...");
            _networkManager.StartHost();
            
            Hosting = true; // above should set this but lets do it here in case
            DependencyManager.Instance.accountManager.HeartbeatNow();
        }

        public void StopHosting()
        {
            Debug.Log("Hosting stopped...");
            _networkManager.StopHost();
            
            Hosting = false; // above should set this but lets do it here in case
            DependencyManager.Instance.accountManager.HeartbeatNow();
        }

        public void Join(string steamId)
        {
            Debug.Log("Joining instance... steam://" + steamId);
            _networkManager.StartClient(new Uri("steam://" + steamId));
        }

        public void Disconnect()
        {
            Debug.Log("Disconnecting...");
            _networkManager.StopClient();
        }

        public void OnGUI()
        {
            if (InWorld)
            {
                if (GUI.Button(new Rect(8, 56, 100, 24), "disconnect"))
                {
                    if (Hosting)
                    {
                        StopHosting();
                    }
                    else
                    {
                        Disconnect();
                    }
                }
            }
        }
    }
}