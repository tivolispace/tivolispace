using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class SteamManager
    {
        private readonly uint _appId = 2161040;
        private readonly bool _initialized;

        public SteamManager()
        {
            try
            {
                if (SteamClient.RestartAppIfNecessary(_appId))
                {
                    Application.Quit();
                    return;
                }

                SteamClient.Init(_appId);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load Steam API\n" + e);
                Application.Quit();
            }
        }

        public void Update()
        {
            if (!_initialized) return;
            SteamClient.RunCallbacks();
        }

        public void OnDestroy()
        {
            if (!_initialized) return;
            SteamClient.Shutdown();
        }

        public SteamId GetMySteamID()
        {
            return SteamClient.SteamId;
        }

        public string GetMyName()
        {
            return SteamClient.Name;
        }

        public Task<Texture2D> GetMyAvatar()
        {
            return GetAvatarTexture(GetMySteamID());
        }

        private readonly Dictionary<ulong, Texture2D> _avatarTextures = new();

        private async Task<Texture2D> GetAvatarTexture(SteamId steamId)
        {
            if (_avatarTextures.TryGetValue(steamId.Value, out var cachedTexture))
            {
                return cachedTexture;
            }


            var image = (await SteamFriends.GetLargeAvatarAsync(steamId)).GetValueOrDefault();

            var texture = new Texture2D((int) image.Width, (int) image.Height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Trilinear
            };

            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var p = image.GetPixel(x, y);
                    texture.SetPixel(x, (int) image.Height - y,
                        new Color(p.r / 255f, p.g / 255f, p.b / 255f, p.a / 255f));
                }
            }
            
            texture.Apply();
            
            _avatarTextures.Add(steamId.Value, texture);
            
            return texture;
        }

        public Task<(string, Texture2D)> GetNameAndAvatar(SteamId steamId)
        {
            var cs = new TaskCompletionSource<(string, Texture2D)>();
            
            async void CompleteGetNameAndAvatar()
            {
                SteamFriends.OnPersonaStateChange -= OnPersonaStateChange;
                var friend = new Friend(steamId);
                var name = friend.Name;
                var avatar = await GetAvatarTexture(steamId);
                cs.SetResult((name, avatar));
            }
            
            void OnPersonaStateChange(Friend friend)
            {
                if (friend.Id.Value == steamId.Value)
                {
                    CompleteGetNameAndAvatar();
                }
            }
            
            SteamFriends.OnPersonaStateChange += OnPersonaStateChange;
            
            var requesting = SteamFriends.RequestUserInformation(steamId, false);
            if (!requesting) CompleteGetNameAndAvatar();

            return cs.Task;
        }

        private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
        {
            Debug.LogWarning(pchDebugText);
        }
    }
}