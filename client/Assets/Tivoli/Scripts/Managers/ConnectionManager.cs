using System;
using System.Threading.Tasks;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;

namespace Tivoli.Scripts.Managers
{
    public class ConnectionManager : Manager
    {
        private readonly AccountManager _accountManager;
        private readonly SteamManager _steamManager;

        private readonly NetworkManager _networkManager;

        public string HostingInstanceId;

        public ConnectionManager(NetworkManager networkManager)
        {
            _accountManager = DependencyManager.Instance.AccountManager;
            _steamManager = DependencyManager.Instance.SteamManager;
            _networkManager = networkManager;
        }

        public override async void OnDestroy()
        {
            await CloseInstance();
            // StopHosting is useless here because its a race condition with Mirror
        }

        public override async Task Init()
        {
            await _steamManager.WhenInitialized();
            await _accountManager.WhenLoggedIn();
            
            var fizzyFacepunch = _networkManager.GetComponent<FizzyFacepunch>();
            fizzyFacepunch.SteamAppID = SteamManager.AppId;
            fizzyFacepunch.SteamUserID = _steamManager.GetMySteamID();
            fizzyFacepunch.Init();
            _networkManager.transport = fizzyFacepunch;

            // create private instance right away
            await StartHosting();
        }

        private async Task CloseInstance()
        {
            if (HostingInstanceId == "") return;
            await _accountManager.CloseInstance(HostingInstanceId);
            HostingInstanceId = "";
        }

        private async Task<bool> StopHostingOrDisconnect()
        {
            if (NetworkServer.active)
            {
                await CloseInstance();

                Debug.Log("Hosting stopped...");
                _networkManager.StopHost();

                _accountManager.HeartbeatNow();

                return true;
            }
            else if (NetworkClient.active)
            {
                Debug.Log("Disconnecting...");
                _networkManager.StopClient();
                
                return true;
            }
            return false;
        }

        public async Task StartHosting()
        {
            var wait = await StopHostingOrDisconnect();
            if (wait)
            {
                await Task.Delay(500);
            }

            HostingInstanceId = await _accountManager.StartInstance("steam://" + _steamManager.GetMySteamID());

            Debug.Log("Hosting started...");
            _networkManager.StartHost();

            _accountManager.HeartbeatNow();
        }

        public async Task Join(string connectionUri)
        {
            var wait = await StopHostingOrDisconnect();
            if (wait)
            {
                await Task.Delay(500);
            }
            
            Debug.Log("Joining instance... " + connectionUri);
            _networkManager.StartClient(new Uri(connectionUri));
        }
    }
}