using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeWebSocket;
using Tivoli.Scripts.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tivoli.Scripts.Managers
{
    public class UserProfile
    {
        public string id;
        public string steamId;
        public string displayName;
        public string profilePictureUrl;
        public Texture2D profilePicture;
    }

    public class AccountManager
    {
#if UNITY_EDITOR
        private static readonly string ApiUrl = EditorPrefs.GetString(TivoliEditorPrefs.OverrideApiUrl);
#else
        private const string ApiUrl = "https://tivoli.space";
#endif

        private string _accessToken;

        public UserProfile Profile;

        private bool _loggedIn;
        private Action _onLoggedIn = () => { };

        private WebSocket _socket;

        private float _heartbeatTimer;
        private const float HeartbeatTime = 10; // seconds
        private bool _heartbeatClosingGame = false;

        public AccountManager()
        {
            Login();
        }

        private async void Login()
        {
            await DependencyManager.Instance.steamManager.WhenInitialized();

            var authTicket = await DependencyManager.Instance.steamManager.GetAuthSessionTicket();

            var (res, error) = await new HttpFox(ApiUrl + "/api/auth/steam-ticket", "POST")
                .WithJson(new {ticket = authTicket})
                .ReceiveJson(new {accessToken = ""});

            if (error != null)
            {
                Debug.Log("Failed to login!\n" + error);
                return;
            }

            Debug.Log("Logged in to: " + ApiUrl);
            _accessToken = res.accessToken;

            // ConnectToWs();
            Heartbeat();

            // get own profile
            Profile = await GetProfile(null);

            _loggedIn = true;
            _onLoggedIn();
        }

        public Task WhenLoggedIn()
        {
            var cs = new TaskCompletionSource<object>();
            if (_loggedIn)
            {
                cs.SetResult(null);
            }
            else
            {
                _onLoggedIn += () => { cs.SetResult(null); };
            }

            return cs.Task;
        }

        private class HeartbeatDto
        {
            public bool hosting;
            public bool closingGame;

            public HeartbeatDto(bool hosting, bool closingGame)
            {
                this.hosting = hosting;
                this.closingGame = closingGame;
            }
        }

        private async void Heartbeat()
        {
            if (_heartbeatClosingGame)
            {
                Debug.Log("Sending heartbeat with close game");
            }

            // TODO: dont send hosting and closingGame every heartbeat

            await new HttpFox(ApiUrl + "/api/user/heartbeat", "PUT")
                .WithBearerAuth(_accessToken)
                .WithJson(new
                {
                    hosting = DependencyManager.Instance.connectionManager.Hosting,
                    closingGame = _heartbeatClosingGame
                })
                .ReceiveNothing();
        }

        private readonly Dictionary<string, Texture2D> _profilePictureCachedTextures = new();

        public async Task<UserProfile> GetProfile(string userId)
        {
            var (userProfile, error) =
                await new HttpFox(ApiUrl + (userId == null ? "/api/user/profile" : "/api/user/profile/" + userId))
                    .WithBearerAuth(_accessToken)
                    .ReceiveJson<UserProfile>();

            if (error != null)
            {
                Debug.LogError("Failed to get profile from user ID: " + userId + "\n" + error);
                return null;
            }

            var profilePictureUrl = userProfile.profilePictureUrl;
            if (!_profilePictureCachedTextures.TryGetValue(profilePictureUrl, out var profilePicture))
            {
                (profilePicture, error) = await new HttpFox(profilePictureUrl).ReceiveTexture();
                if (error != null)
                {
                    Debug.LogError(
                        "Failed to get profile picture from user ID: " + userId +
                        "(will use default)\n" + error
                    );
                    // TODO: go set to use default lol
                }

                _profilePictureCachedTextures[profilePictureUrl] = profilePicture;
            }

            userProfile.profilePicture = profilePicture;

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
            var (allOnlineUsers, _) = await new HttpFox(ApiUrl + "/api/stats/online")
                .WithBearerAuth(_accessToken)
                .ReceiveJson<AllOnlineUsers>();

            return allOnlineUsers;
        }

        public void Update()
        {
            if (_loggedIn)
            {
                _heartbeatTimer += Time.deltaTime;
                if (_heartbeatTimer > HeartbeatTime)
                {
                    _heartbeatTimer = 0f;
                    Heartbeat();
                }
            }
        }

        public void HeartbeatNow()
        {
            _heartbeatTimer = 0f;
            Heartbeat();
        }

        public void OnDestroy()
        {
            _heartbeatClosingGame = true;
            HeartbeatNow();
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