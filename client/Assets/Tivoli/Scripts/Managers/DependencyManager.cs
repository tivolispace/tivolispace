using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Tivoli.Scripts.Managers
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        public WindowManager WindowManager;

        public SteamManager SteamManager;

        public AccountManager AccountManager;

        [Header("Connection Manager")] public TivoliNetworkManager connectionNetworkManager;
        public ConnectionManager ConnectionManager;

        public VRManager VRManager;
        
        [Header("UI Manager")] public Canvas uiCanvas;
        public Camera uiMainCamera;
        public GameObject uiMainMenu;
        public UIManager UIManager;

        [Header("Other")] [Scene] public string loadingScene;
        public List<GameObject> persistantGameObjects;

        private Manager[] _managers;
        private bool _initialized;

        private async void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            var managers = new List<Manager>();

            managers.Add(WindowManager = new WindowManager());
            managers.Add(SteamManager = new SteamManager());
            managers.Add(AccountManager = new AccountManager());

            managers.Add(ConnectionManager = new ConnectionManager(connectionNetworkManager));
            persistantGameObjects.Add(connectionNetworkManager.gameObject);

            managers.Add(VRManager = new VRManager());
            
            managers.Add(UIManager = new UIManager(uiMainCamera, uiCanvas, uiMainMenu));
            persistantGameObjects.Add(uiMainCamera.gameObject);
            persistantGameObjects.Add(uiCanvas.gameObject);

            _managers = managers.ToArray();

            DontDestroyOnLoad();

            // switch to loading and start initializing!
            SceneManager.LoadScene(loadingScene);

            await Task.WhenAll(_managers.Select(m => m.Init()));

            _initialized = true;
        }

        public void DontDestroyOnLoad()
        {
            foreach (var persistantGameObject in persistantGameObjects)
            {
                DontDestroyOnLoad(persistantGameObject);
            }
        }

        public void Update()
        {
            if (!_initialized) return;

            foreach (var manager in _managers)
            {
                manager.Update();
            }
        }

        public void OnDestroy()
        {
            if (!_initialized) return;

            foreach (var manager in _managers)
            {
                manager.OnDestroy();
            }
        }

        // public void OnGUI()
        // {
        //     if (!_initialized) return;
        // }
    }
}