using Mirror.FizzySteam;
using Steamworks;
using Tivoli.Scripts.Networking;
using UnityEngine;

namespace Tivoli.Scripts.Managers
{
    public class ConnectionManager
    {
        private TivoliNetworkManager _networkManager;
        private FizzyFacepunch _fizzyFacepunch;

        // TivoliNetworkManager sets this value
        public bool Hosting;

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

        public void Host()
        {
            Debug.Log("Hosting started...");
            _networkManager.StartHost();
            Hosting = true; // above should set this but lets do it here in case
            DependencyManager.Instance.accountManager.HeartbeatNow();
        }
    }
}