using System;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace Tivoli.Scripts.Managers
{
    public class SteamManager : Manager
    {
        public const uint AppId = 2161040;

        private bool _initialized;
        private Action _onInitialized = () => { };

        public SteamManager()
        {
        }

        public override Task Init()
        {
            try
            {
                // will restart from steam
                // if (SteamClient.RestartAppIfNecessary(AppId))
                // {
                //     Application.Quit();
                //     return;
                // }

                SteamClient.Init(AppId);

                _initialized = true;
                _onInitialized.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load Steam API\n" + e);
                Application.Quit();
            }

            return Task.CompletedTask;
        }

        public Task WhenInitialized()
        {
            var cs = new TaskCompletionSource<object>();
            if (_initialized)
            {
                cs.SetResult(null);
            }
            else
            {
                _onInitialized += () => { cs.SetResult(null); };
            }

            return cs.Task;
        }

        public override void Update()
        {
            if (!_initialized) return;
            SteamClient.RunCallbacks();
        }

        public override void OnDestroy()
        {
            if (!_initialized) return;
            SteamClient.Shutdown();
        }

        public SteamId GetMySteamID()
        {
            return SteamClient.SteamId;
        }

        public async Task<string> GetAuthSessionTicket()
        {
            var authTicket = await SteamUser.GetAuthSessionTicketAsync();
            var authTicketHex = BitConverter.ToString(authTicket.Data).Replace("-", "");
            return authTicketHex;
        }
    }
}