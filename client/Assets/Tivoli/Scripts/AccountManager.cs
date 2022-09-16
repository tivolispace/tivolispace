using System;
using UnityEngine.Device;

namespace Tivoli.Scripts
{
    public class AccountManager
    {
        private string _username;
        
        public AccountManager()
        {
            _username = (SystemInfo.deviceName + new Random().Next(0, 999)).ToLower();
        }
        
        public string GetUsername()
        {
            return _username;
        }
    }
}