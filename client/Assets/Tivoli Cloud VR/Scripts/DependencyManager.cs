using DefaultNamespace;
using Mirror;
using UnityEngine;

public class DependencyManager : MonoBehaviour
{
    public static DependencyManager Instance;

    public WindowManager windowManager;
    public NetworkManager networkManager; // add in inspect
    
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
