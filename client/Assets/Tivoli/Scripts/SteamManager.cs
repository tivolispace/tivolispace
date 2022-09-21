using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class SteamManager
    {
        private readonly AppId_t _appId = new(2161040);
        private readonly bool _initialized;
        
        public SteamManager()
        {
            if (!Packsize.Test()) {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            }

            if (!DllCheck.Test()) {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            }
            
            try {
                if (SteamAPI.RestartAppIfNecessary(_appId)) {
                    Application.Quit();
                    return;
                }
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);
                Application.Quit();
                return;
            }

            _initialized = SteamAPI.Init();
            if (!_initialized) {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
                return;
            }
            
            var steamApiWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(steamApiWarningMessageHook);
            
            Debug.Log("Steamworks.NET successfully initialized");
        }

        public CSteamID GetMySteamID()
        {
            return SteamUser.GetSteamID();
        }

        public string GetMyName()
        {
            return SteamFriends.GetPersonaName();
        }
        
        public Texture2D GetMyAvatar()
        {
            return GetAvatarTexture(GetMySteamID());
        }

        private readonly Dictionary<ulong, Texture2D> _avatarTextures = new();
        private Texture2D GetAvatarTexture(CSteamID steamId)
        {
            if (_avatarTextures.TryGetValue(steamId.m_SteamID, out var cachedTexture))
            {
                return cachedTexture;
            }
            
            var iImage = SteamFriends.GetLargeFriendAvatar(steamId);
            
            var isValid = SteamUtils.GetImageSize(iImage,  out var imageWidth, out var imageHeight);
            if (!isValid) return null;
            
            var image = new byte[imageWidth * imageHeight * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int) (imageWidth * imageHeight * 4));
            if (!isValid) return null;
            
            Debug.Log(imageWidth);
            Debug.Log(imageHeight);
            
            var imageFlipped = new byte[imageWidth * imageHeight * 4];
            for (var y = 0; y < imageHeight; y++)
            {
                for (var x = 0; x < imageWidth; x++)
                {
                    var index = (y * imageWidth + x) * 4;
                    var indexFlipped = ((imageHeight - y - 1) * imageWidth + x) * 4;
                    imageFlipped[indexFlipped] = image[index];
                    imageFlipped[indexFlipped + 1] = image[index + 1];
                    imageFlipped[indexFlipped + 2] = image[index + 2];
                    imageFlipped[indexFlipped + 3] = image[index + 3];
                }
            }
            
            var texture = new Texture2D((int) imageWidth, (int) imageHeight, TextureFormat.RGBA32, false, false);
            texture.LoadRawTextureData(imageFlipped);
            texture.Apply();
            
            _avatarTextures.Add(steamId.m_SteamID, texture);
            return texture;
        }

        public void GetNameAndAvatar(CSteamID steamId, Action<string, Texture2D> onNameAndAvatar)
        {
            Callback<PersonaStateChange_t> personaStateChange = null;
            
            var getNameAndAvatar = new Action(() =>
            {
                personaStateChange?.Dispose();
                var name = SteamFriends.GetFriendPersonaName(steamId);
                var avatar = GetAvatarTexture(steamId);
                onNameAndAvatar.Invoke(name, avatar);
            });
            
            personaStateChange = new Callback<PersonaStateChange_t>(e =>
            {
                if (e.m_ulSteamID == steamId.m_SteamID)
                {
                    getNameAndAvatar();
                }
            });
            
            var requesting = SteamFriends.RequestUserInformation(steamId, false);
            if (!requesting) getNameAndAvatar();
            
        }

        private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText) {
            Debug.LogWarning(pchDebugText);
        }

        public void Update()
        {
            if (!_initialized) return;
            
            SteamAPI.RunCallbacks();
        }

        public void OnDestroy()
        {
            if (!_initialized) return;
            
            SteamAPI.Shutdown();
        }
    }
}
