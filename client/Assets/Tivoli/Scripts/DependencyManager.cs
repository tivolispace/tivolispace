using System;
using UnityEngine;

namespace Tivoli.Scripts
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        public WindowManager windowManager;
        public SteamManager steamManager;
        public AccountManager accountManager;

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
            steamManager = new SteamManager();
            accountManager = new AccountManager();
            
            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized) return;
            
            steamManager.Update();
            // accountManager.Update();
        }

        public void OnDestroy()
        {
            if (!_initialized) return;
            
            steamManager.OnDestroy();
        }
    }
    
}
