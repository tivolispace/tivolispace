using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tivoli.Scripts.Managers
{
    public class UIManager : Manager
    {
        private readonly Camera _uiCamera;
        private readonly Canvas _uiCanvas;
        private readonly GameObject _uiMainMenu;

        public UIManager(Camera uiMainCamera, Canvas uiCanvas, GameObject uiMainMenu)
        {
            _uiCamera = uiMainCamera;
            _uiCanvas = uiCanvas;
            _uiMainMenu = uiMainMenu;
        }

        public override Task Init()
        {
            var inputActions = new TivoliInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            inputActions.Player.ToggleMainMenu.Enable();
            inputActions.Player.ToggleMainMenu.performed += OnToggleMainMenu;

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

        private void OnToggleMainMenu(InputAction.CallbackContext obj)
        {
            if (!_uiMainMenu.activeSelf)
            {
                var cameraPosition = _uiCamera.transform.position;
                var cameraRotation = _uiCamera.transform.rotation;

                var canvas = _uiCanvas.GetComponent<RectTransform>();
                canvas.position = cameraPosition +
                                  Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f) *
                                  new Vector3(0f, -0.4f, 1f);

                canvas.rotation = Quaternion.Euler(10f, cameraRotation.eulerAngles.y, 0f);

                // TODO: refresh main menu when opening
                _uiMainMenu.SetActive(true);
            }
            else
            {
                _uiMainMenu.SetActive(false);
            }
        }

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