using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Tivoli.Scripts.Managers
{
    public class MainMenuManager
    {
        public MainMenuManager()
        {
            var inputActions = new TivoliInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            inputActions.Player.ToggleMainMenu.Enable();
            inputActions.Player.ToggleMainMenu.performed += ToggleMainMenu;
        }

        private static void ToggleMainMenu(InputAction.CallbackContext obj = default)
        {
            try
            {
                // TODO: DONT DO THIS, there's just no way to assign things to managers yet
                var objects = Object.FindObjectsOfType<RectTransform>(true);
                var mainMenu = objects.First(transform => transform.CompareTag("MainMenu"));
                mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
            }
            catch (Exception _)
            {
                // ignored
            }
        }
    }
}