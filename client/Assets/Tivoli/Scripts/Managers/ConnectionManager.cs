using System;
using kcp2k;
using Tivoli.Scripts.Networking;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tivoli.Scripts.Managers
{
    public class ConnectionManager
    {
        private TivoliNetworkManager _networkManager;

        // private FizzyFacepunch _fizzyFacepunch;
        private KcpTransport _kcpTransport;

        private TivoliHolepunch _holepunch;

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

            // _fizzyFacepunch = _networkManager.GetComponent<FizzyFacepunch>();
            // _fizzyFacepunch.SteamAppID = SteamManager.AppId;
            // _fizzyFacepunch.SteamUserID = steamManager.GetMySteamID();
            // _fizzyFacepunch.Init();

            _kcpTransport = _networkManager.GetComponent<KcpTransport>();

            _holepunch = new TivoliHolepunch();
        }

        public async void StartHosting()
        {
            if (Hosting) return;

            var accountManager = DependencyManager.Instance.accountManager;

            var instanceId = await accountManager.StartInstance("there is none");

            var myEndpoint = await _holepunch.StartHost(instanceId);
            _kcpTransport.Port = (ushort) myEndpoint.Port;

            Debug.Log("Hosting started...");
            _networkManager.StartHost();

            Hosting = true; // above should set this but lets do it here in case
            HostingInstanceId = instanceId;

            accountManager.HeartbeatNow();
        }

        public async void StopHosting()
        {
            if (!Hosting) return;

            _holepunch.StopHost();

            await DependencyManager.Instance.accountManager.CloseInstance(HostingInstanceId);

            Debug.Log("Hosting stopped...");
            _networkManager.StopHost();

            Hosting = false; // above should set this but lets do it here in case
            HostingInstanceId = "";

            DependencyManager.Instance.accountManager.HeartbeatNow();
        }

        public async void Join(string instanceId)
        {
            Debug.Log("UDP hole punching instance... " + instanceId);

            var (myEndpoint, hostEndpoint) = await _holepunch.StartClient(instanceId);
            _kcpTransport.BindPort = (ushort) myEndpoint.Port;
            
            Debug.Log("Joining instance... " + hostEndpoint);
            _networkManager.StartClient(new Uri("kcp://" + hostEndpoint.Address + ":" + hostEndpoint.Port));

        }

        public void Disconnect()
        {
            Debug.Log("Disconnecting...");
            _holepunch.StopClient();
            _networkManager.StopClient();
        }

        public void OnDestroy()
        {
            StopHosting();
            _holepunch.OnDestroy();
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