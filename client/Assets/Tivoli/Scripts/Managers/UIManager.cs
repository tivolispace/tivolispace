using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tivoli.Scripts.Managers
{
    public class UIManager : Manager
    {
        private readonly Camera _uiMainCamera;
        private readonly Canvas _uiCanvas;
        private readonly GameObject _uiMainMenu;

        public UIManager(Camera uiMainCamera, Canvas uiCanvas, GameObject uiMainMenu)
        {
            _uiMainCamera = uiMainCamera;
            _uiCanvas = uiCanvas;
            _uiMainMenu = uiMainMenu;
        }

        public override Task Init()
        {
            var inputActions = new TivoliInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            inputActions.Player.ToggleMainMenu.Enable();
            inputActions.Player.ToggleMainMenu.performed += _ =>
            {
                _uiMainMenu.SetActive(!_uiMainMenu.activeSelf);
            };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Loading")
                {
                    var transform = _uiMainCamera.transform;
                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;
                }
            };

            return Task.CompletedTask;
        }

        public Camera GetMainCamera()
        {
            return _uiMainCamera;
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