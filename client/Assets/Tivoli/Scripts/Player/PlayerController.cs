﻿using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tivoli.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public Transform cameraBoom;
        
        private Rigidbody _playerRigidbody;
        
        private InputActions _inputActions;
        
        private bool _mouseLocked;

        public void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
        }
        
        public override void OnStartLocalPlayer()
        {
            _inputActions = new InputActions();
            _inputActions.Player.Enable();
            _inputActions.Player.Move.Enable();
            _inputActions.Player.Look.Enable();
            _inputActions.Player.Look.performed += OnLook;
            _inputActions.Player.ActivateLook.Enable();

            // attach main camera
            Camera.main.transform.parent = cameraBoom;
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -2f);
            cameraBoom.transform.eulerAngles = new Vector3(10f, 0f, 0f);
        }

        public override void OnStopLocalPlayer()
        {
            // detach main camera
            Camera.main.transform.parent = null;
        }
        
        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            
            var moveXy = _inputActions.Player.Move.ReadValue<Vector2>();
            var positionOffset = Quaternion.Euler(0, transform.localEulerAngles.y, 0) *
                                 new Vector3(moveXy.x, 0, moveXy.y);
            _playerRigidbody.MovePosition(transform.position + positionOffset * 0.1f);
        }
        
        private void LockMouse()
        {
            if (_mouseLocked) return;
            Cursor.lockState = CursorLockMode.Locked;
            _mouseLocked = true;
        }
        
        private void UnlockMouse()
        {
            if (!_mouseLocked) return;
            Cursor.lockState = CursorLockMode.None;
            _mouseLocked = false;
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            
            var activateLook = _inputActions.Player.ActivateLook.ReadValue<float>() > 0.5f;
            if (activateLook)
            {
                LockMouse();
            }
            else
            {
                UnlockMouse();
                return;
            }
            
            const float sensitivity = 0.25f;
        
            var lookDelta = _inputActions.Player.Look.ReadValue<Vector2>();
            transform.localEulerAngles += new Vector3(0f, lookDelta.x * sensitivity, 0f);
        
            var newCameraBoom = cameraBoom.localEulerAngles + new Vector3(-lookDelta.y * sensitivity, 0f, 0f);
        
            // 90deg
            // --.
            //    | 0deg
            //    | 360deg
            // --`
            // 270deg
            if (newCameraBoom.x is > 90 and < 180) newCameraBoom.x = 90;
            if (newCameraBoom.x is < 270 and > 180) newCameraBoom.x = 270;
        
            cameraBoom.localEulerAngles = newCameraBoom;
        }
    }
}
