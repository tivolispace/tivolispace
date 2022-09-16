using UnityEngine;

namespace Tivoli.Scripts
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        public WindowManager windowManager;
        public AccountManager accountManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            windowManager = new WindowManager();
            accountManager = new AccountManager();
            
            DontDestroyOnLoad(gameObject);
        }
    }
}
