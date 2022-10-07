using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tivoli.Scripts.Managers
{
    public class UIManager : Manager
    {
        // private readonly XROrigin _uiXrOrigin;
        private readonly Camera _uiCamera;

        private readonly Canvas _uiCanvas;
        private readonly GameObject _uiMainMenu;

        public UIManager(Canvas uiCanvas, GameObject uiMainMenu)
        {
            // _uiXrOrigin = uiXrOrigin;
            // _uiCamera = uiXrOrigin.Camera;

            _uiCanvas = uiCanvas;
            _uiMainMenu = uiMainMenu;
        }

        public override Task Init()
        {
            var inputActions = new TivoliInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            inputActions.Player.ToggleMainMenu.Enable();
            inputActions.Player.ToggleMainMenu.performed += _ => { _uiMainMenu.SetActive(!_uiMainMenu.activeSelf); };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Loading")
                {
                    var transform = _uiCamera.transform;
                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;
                }
            };

            return Task.CompletedTask;
        }

        // public XROrigin GetXrOrigin()
        // {
        //     return _uiXrOrigin;
        // }

        public Camera GetMainCamera()
        {
            return _uiCamera;
        }

        public async Task StartHosting()
        {
            _uiMainMenu.SetActive(false);
            await DependencyManager.Instance.ConnectionManager.StartHosting();
        }

        public async Task Join(string connectionUri)
        {
            _uiMainMenu.SetActive(false);
            await DependencyManager.Instance.ConnectionManager.Join(connectionUri);
        }
    }
}