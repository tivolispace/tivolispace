using System;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using System.Threading;
using Newtonsoft.Json;
using Steamworks;
using Tivoli.Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Tivoli.Scripts
{
    public class AccountManager
    {
        private const string ApiUrl = "http://127.0.0.1:3000";
        // private const string ApiUrl = "https://tivoli.space";

        private string _accessToken;

        public bool LoggedIn;
        public Action OnLoggedIn = () => { };

        private WebSocket _socket;

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

                // ConnectToWs();
            }
            else
            {
                Debug.Log(req.error);
            }
        }

        /*
        private class WebsocketEvent
        {
            public string @event;
            public Dictionary<string, string> data;
        }

        private async void SendToWs(string eventName, Dictionary<string, string> data)
        {
            if (_socket is not {State: WebSocketState.Open}) return;

            var eventMessage = new WebsocketEvent
            {
                @event = eventName,
                data = data
            };

            var messageJson = JsonConvert.SerializeObject(eventMessage);
            Debug.Log(messageJson);

            await _socket.SendText(messageJson);
        }

        private async void ConnectToWs()
        {
            // http:// => ws:// and https:// => wss:// 
            _socket = new WebSocket(ApiUrl.Replace("http", "ws"));

            _socket.OnOpen += () =>
            {
                Debug.Log("Connected to backend socket");
                SendToWs("auth", new Dictionary<string, string>()
                {
                    {"accessToken", _accessToken}
                });
            };

            _socket.OnError += (e) => { Debug.Log("Error! " + e); };

            _socket.OnClose += (e) => { Debug.Log("Connection closed!"); };

            _socket.OnMessage += (bytes) =>
            {
                Debug.Log("OnMessage!");
                Debug.Log(bytes);

                // getting the message as a string
                // var message = System.Text.Encoding.UTF8.GetString(bytes);
                // Debug.Log("OnMessage! " + message);
            };

            // InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

            // waiting for messages
            await _socket.Connect();
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _socket?.DispatchMessageQueue();
#endif
        }
        */
    }
}