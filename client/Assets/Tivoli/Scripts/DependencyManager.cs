using System;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        public WindowManager windowManager;
        // public AccountManager accountManager;
        public SteamManager steamManager;

        private bool _initialized;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            windowManager = new WindowManager();
            // accountManager = new AccountManager();
            steamManager = new SteamManager();
            
            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized) return;
            
            steamManager.Update();
        }

        public void OnDestroy()
        {
            if (!_initialized) return;
            
            steamManager.OnDestroy();
        }
    }
    
}
