using System;
using System.Threading.Tasks;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tivoli.Scripts.Managers
{
    public class ConnectionManager
    {
        private NetworkManager _networkManager;
        private FizzyFacepunch _fizzyFacepunch;

        public string HostingInstanceId;

        public ConnectionManager()
        {
            Init();
        }

        public async void OnDestroy()
        {
            await CloseInstance();
            // StopHosting is useless here because its a race condition with Mirror
        }

        private async void Init()
        {
            var steamManager = DependencyManager.Instance.steamManager;
            await steamManager.WhenInitialized();

            await DependencyManager.Instance.accountManager.WhenLoggedIn();

            _networkManager = Object.FindObjectOfType<NetworkManager>();

            _fizzyFacepunch = _networkManager.GetComponent<FizzyFacepunch>();
            _fizzyFacepunch.SteamAppID = SteamManager.AppId;
            _fizzyFacepunch.SteamUserID = steamManager.GetMySteamID();
            _fizzyFacepunch.Init();

            // create private instance right away
            await StartHosting();
        }

        public async Task StartHosting()
        {
            if (NetworkServer.active) return;

            var accountManager = DependencyManager.Instance.accountManager;

            HostingInstanceId =
                await accountManager.StartInstance("steam://" + DependencyManager.Instance.steamManager.GetMySteamID());

            Debug.Log("Hosting started...");
            _networkManager.StartHost();

            accountManager.HeartbeatNow();
        }

        private async Task CloseInstance()
        {
            if (HostingInstanceId == "") return;
            await DependencyManager.Instance.accountManager.CloseInstance(HostingInstanceId);
            HostingInstanceId = "";
        }

        public async Task StopHosting()
        {
            if (!NetworkServer.active) return;

            await CloseInstance();

            Debug.Log("Hosting stopped...");
            _networkManager.StopHost();
            
            DependencyManager.Instance.accountManager.HeartbeatNow();
        }

        public void Join(string connectionUri)
        {
            Debug.Log("Joining instance... " + connectionUri);
            _networkManager.StartClient(new Uri(connectionUri));
        }

        public void Disconnect()
        {
            Debug.Log("Disconnecting...");
            _networkManager.StopClient();
        }

        public void OnGUI()
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                if (GUI.Button(new Rect(8, 56, 100, 24), "disconnect"))
                {
                    if (NetworkServer.active)
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