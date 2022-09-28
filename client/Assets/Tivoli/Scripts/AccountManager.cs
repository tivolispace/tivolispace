using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeWebSocket;
using Steamworks;
using Tivoli.Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Tivoli.Scripts
{
    public class UserProfile
    {
        public string Id;
        public string SteamId;
        public string DisplayName;
        public Texture2D ProfilePicture;
    }

    public class AccountManager
    {
        private const string ApiUrl = "http://127.0.0.1:3000";
        // private const string ApiUrl = "https://tivoli.space";

        private string _accessToken;

        public UserProfile Profile;

        public bool LoggedIn;
        public Action OnLoggedIn = () => { };

        private WebSocket _socket;

        private float _heartbeatTimer;
        private const float HeartbeatTime = 10; // seconds

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

            var success = res.TryGetValue("accessToken", out var accessToken);
            if (success == false)
            {
                Debug.Log(req.error);
                return;
            }

            Debug.Log("Logged in!");
            _accessToken = accessToken;

            // ConnectToWs();
            Heartbeat();

            // get own profile
            Profile = await GetProfile(null);

            LoggedIn = true;
            OnLoggedIn();
        }

        private async void Heartbeat()
        {
            var (req, res) = await HttpRequest.Simple(new Dictionary<string, string>()
            {
                {"method", "PUT"},
                {"url", ApiUrl + "/api/user/heartbeat"},
                {"auth", _accessToken}
            });
        }

        private readonly Dictionary<string, Texture2D> _profilePictureCachedTextures = new();

        public async Task<UserProfile> GetProfile(string userId)
        {
            var (req, res) = await HttpRequest.Simple(new Dictionary<string, string>()
            {
                {"method", "GET"},
                {"url", ApiUrl + (userId == null ? "/api/user/profile" : "/api/user/profile/" + userId)},
                {"auth", _accessToken}
            });

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to get profile with user ID: " + userId + "\n" + req.error);
                return null;
            }

            var profilePictureUrl = res.GetValueOrDefault("profilePictureUrl", "");
            if (!_profilePictureCachedTextures.TryGetValue(profilePictureUrl, out var profilePicture))
            {
                profilePicture = await HttpRequest.Texture(profilePictureUrl);
                _profilePictureCachedTextures[profilePictureUrl] = profilePicture;
            }

            var userProfile = new UserProfile
            {
                Id = res.GetValueOrDefault("id", ""),
                SteamId = res.GetValueOrDefault("steamId", ""),
                DisplayName = res.GetValueOrDefault("displayName", ""),
                ProfilePicture = profilePicture
            };

            return userProfile;
        }

        [Serializable]
        public class AllOnlineUsers
        {
            public int count;
            public string[] userIds;
        }

        public async Task<AllOnlineUsers> GetAllOnlineUsers()
        {
            var (req, _) = await HttpRequest.Simple(new Dictionary<string, string>()
            {
                {"method", "GET"},
                {"url", ApiUrl + "/api/stats/online"},
                {"auth", _accessToken}
            }, new Dictionary<string, string>(), false);

            var json = req.downloadHandler.text;
            
            return JsonUtility.FromJson<AllOnlineUsers>(json);
        }

        public void Update()
        {
            if (LoggedIn)
            {
                _heartbeatTimer += Time.deltaTime;
                if (_heartbeatTimer > HeartbeatTime)
                {
                    _heartbeatTimer = 0f;
                    Heartbeat();
                }
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