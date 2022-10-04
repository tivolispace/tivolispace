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
        private static readonly string ApiUrl =
            Environment.GetEnvironmentVariable("OVERRIDE_API_URL") ?? "https://tivoli.space";
#endif

        private string _accessToken;

        public UserProfile Profile;

        private bool _loggedIn;
        private Action _onLoggedIn = () => { };

        private WebSocket _socket;

        private float _heartbeatTimer;
        private const float HeartbeatTime = 10; // seconds
        private bool _heartbeatClosingGame = false;

        private readonly Dictionary<string, Texture2D> _cachedUrlTextures = new();

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
                    hostingInstanceId = DependencyManager.Instance.connectionManager.HostingInstanceId,
                    closingGame = _heartbeatClosingGame
                })
                .ReceiveNothing();
        }

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
            if (!_cachedUrlTextures.TryGetValue(profilePictureUrl, out var profilePicture))
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

                _cachedUrlTextures[profilePictureUrl] = profilePicture;
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

        [Serializable]
        public class InstanceOwner
        {
            public string id;
            public string displayName;
            public string profilePictureUrl;
        }

        [Serializable]
        public class Instance
        {
            public string id;
            public InstanceOwner owner;
            public string transport;
            public string connectionUri;
        }

        [Serializable]
        public class AllInstances
        {
            public Instance[] instances;
        }

        public async Task<Instance[]> GetAllInstances()
        {
            var (res, error) = await new HttpFox(ApiUrl + "/api/instance/all")
                .WithBearerAuth(_accessToken)
                .ReceiveJson<AllInstances>();


            if (error != null)
            {
                Debug.LogError("Failed to get all instances");
                return new Instance[] { };
            }

            return res.instances;
        }

        public async Task<string> StartInstance(string connectionUri)
        {
            Debug.Log("Starting instance...");

            var (res, error) = await new HttpFox(ApiUrl + "/api/instance/start", "POST")
                .WithBearerAuth(_accessToken)
                .WithJson(new
                {
                    connectionUri,
                })
                .ReceiveJson(new {id = ""});

            if (error != null)
            {
                Debug.LogError("Failed to start instance\n" + error);
                return null;
            }

            return res.id;
        }

        public async Task CloseInstance(string instanceId)
        {
            Debug.Log("Closing instance...");

            await new HttpFox(ApiUrl + "/api/instance/close/" + instanceId, "POST")
                .WithBearerAuth(_accessToken)
                .ReceiveNothing();
        }

        public void HeartbeatNow()
        {
            _heartbeatTimer = 0f;
            Heartbeat();
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