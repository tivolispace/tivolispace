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

        // TivoliNetworkManager sets these values
        public bool Hosting;
        public bool InWorld;

        public string HostingInstanceId;

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

        public async void StartHosting()
        {
            if (Hosting) return;

            var accountManager = DependencyManager.Instance.accountManager;

            var instanceId =
                await accountManager.StartInstance("steam://" + DependencyManager.Instance.steamManager.GetMySteamID());

            Debug.Log("Hosting started...");
            _networkManager.StartHost();

            Hosting = true; // above should set this but lets do it here in case
            HostingInstanceId = instanceId;

            accountManager.HeartbeatNow();
        }

        public async void StopHosting()
        {
            if (!Hosting) return;

            await DependencyManager.Instance.accountManager.CloseInstance(HostingInstanceId);

            Debug.Log("Hosting stopped...");
            _networkManager.StopHost();

            Hosting = false; // above should set this but lets do it here in case
            HostingInstanceId = "";

            DependencyManager.Instance.accountManager.HeartbeatNow();
        }

        public async void Join(string connectionUri)
        {
            Debug.Log("Joining instance... " + connectionUri);
            _networkManager.StartClient(new Uri(connectionUri));
        }

        public void Disconnect()
        {
            Debug.Log("Disconnecting...");
            _networkManager.StopClient();
        }

        public void OnDestroy()
        {
            StopHosting();
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