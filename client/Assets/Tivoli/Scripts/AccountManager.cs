using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Device;

namespace Tivoli.Scripts
{
    public class AccountManager
    {
        private readonly string _username;
        
        public AccountManager()
        {
            _username = (SystemInfo.deviceName + new Random().Next(0, 999)).ToLower();
        }
        
        public string GetMyUsername()
        {
            return _username;
        }
        
        public string GetProfilePictureUrl(string username)
        {
            using var hash = MD5.Create();
            var data = hash.ComputeHash(Encoding.UTF8.GetBytes(username));
            var stringBuilder = new StringBuilder();
            foreach (var b in data) stringBuilder.Append(b.ToString("x2"));
            var hashString = stringBuilder.ToString();
            return $"https://avatars.dicebear.com/api/identicon/{hashString}.png?backgroundColor=white";
        }
    }
}