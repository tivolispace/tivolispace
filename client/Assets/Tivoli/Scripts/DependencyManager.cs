using UnityEngine;

namespace Tivoli.Scripts
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        public WindowManager windowManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            windowManager = new WindowManager();
        }
    }
}
