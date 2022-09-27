using System;
using System.Collections.Generic;
using Steamworks;
using Tivoli.Scripts.Utils;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class AccountManager
    {
        // private const string ApiUrl = "http://127.0.0.1:3000";
        private const string ApiUrl = "https://tivoli.space";

        private string _accessToken;

        public bool LoggedIn;
        public Action OnLoggedIn = () => { };

        public AccountManager()
        {
            var steamManager = DependencyManager.Instance.steamManager;

            if (steamManager.Initialized)
            {
                Login();
            }
            else
            {
                steamManager.OnInitialized += Login;
            }
        }

        private async void Login()
        {
            var authTicket = await SteamUser.GetAuthSessionTicketAsync();
            var authTicketHex = BitConverter.ToString(authTicket.Data).Replace("-", "");

            var (req, res) = await HttpRequest.Simple(
                new Dictionary<string, string>
                {
                    {"method", "POST"},
                    {"url", ApiUrl + "/api/auth/steam-ticket"},
                },
                new Dictionary<string, string>
                {
                    {"ticket", authTicketHex}
                }
            );

            if (res.TryGetValue("accessToken", out var accessToken))
            {
                Debug.Log("Access token! " + accessToken);
                _accessToken = accessToken;
                LoggedIn = true;
                OnLoggedIn();
            }
            else
            {
                Debug.Log(req.error);
            }
        }
    }
}