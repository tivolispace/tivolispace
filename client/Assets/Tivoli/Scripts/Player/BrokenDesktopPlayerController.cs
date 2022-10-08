using Mirror;
using Tivoli.Scripts.Managers;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class BrokenDesktopPlayerController : NetworkBehaviour
    {
        // public Transform cameraBoom;
        public Transform playerOffset;

        // private XROrigin _xrOrigin;
        private Camera _mainCamera;

        private Rigidbody _playerRigidbody;
        // private CharacterController _characterController;

        private TivoliInputActions _inputActions;

        private readonly TweenManager _tweenManager = new();
        private TweenManager.Tweener _cameraBoomTweener;

        // private const float CameraBoomInitial = 2f;
        // private const float CameraBoomMinimumDistance = 1f;
        //
        // private bool _firstPerson = false;

        private bool _mouseLocked;

        public void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            // _characterController = GetComponent<CharacterController>();
        }

        public override void OnStartLocalPlayer()
        {
            _inputActions = new TivoliInputActions();
            _inputActions.Player.Enable();
            _inputActions.Player.Move.Enable();
            // _inputActions.Player.ThirdPersonLook.Enable();
            // _inputActions.Player.ThirdPersonLook.performed += OnThirdPersonLook;
            // _inputActions.Player.ThirdPersonBoomLength.performed += OnThirdPersonBoomLength;
            // _inputActions.Player.ThirdPersonActivateLook.Enable();

            // move xr rig to player, then player will update follow camera around  
            // _xrOrigin = DependencyManager.Instance.UIManager.GetXrOrigin();
            // _xrRigTransform.position = transform.position;
            // _xrRigTransform.rotation = transform.rotation; // TODO: make sure its flat
            // _xrRigTransform.parent = transform;

            _mainCamera = DependencyManager.Instance.UIManager.GetMainCamera();

            /*
            // attach main camera
            var camera = DependencyManager.Instance.UIManager.GetMainCamera();
            camera.transform.parent = cameraBoom;
            camera.transform.eulerAngles = Vector3.zero;

            _cameraBoomTweener = _tweenManager.NewTweener(SetCameraBoomLength, CameraBoomInitial);

            // rotate down a little
            cameraBoom.transform.eulerAngles = new Vector3(10f, 0f, 0f);
            */
        }

        // private void SetCameraBoomLength(float length)
        // {
        //     if (!isLocalPlayer) return;
        //     DependencyManager.Instance.UIManager.GetMainCamera().transform.localPosition =
        //         new Vector3(0f, 0f, _firstPerson ? 0 : -length);
        // }

        public override void OnStopLocalPlayer()
        {
            _inputActions.Dispose();
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                _tweenManager.Update();
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            return;

            // TODO: look into character controller some day

            // var moveXy = _inputActions.Player.Move.ReadValue<Vector2>();
            //  
            // // _mainCamera.transform.eulerAngles.y
            //
            // var positionOffset = Quaternion.Euler(0, transform.localEulerAngles.y, 0) *
            //                      new Vector3(moveXy.x, 0, moveXy.y);
            //
            // _playerRigidbody.MovePosition(transform.position + positionOffset * 0.1f);
            
            // _characterController.Move(moveXy);

            // transform.
            //
            // _characterController.center = _mainCamera.transform.position - transform.position;
            // _characterController.height = _mainCamera.transform.position.y;
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

        /*
        private void OnThirdPersonLook(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;

            var activateLook = _inputActions.Player.ThirdPersonActivateLook.ReadValue<float>() > 0.5f;
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

            var lookDelta = _inputActions.Player.ThirdPersonLook.ReadValue<Vector2>();
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

        private void OnThirdPersonBoomLength(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;

            var delta = _inputActions.Player.ThirdPersonBoomLength.ReadValue<float>();
            var zoomOut = delta < 0;

            if (_firstPerson && zoomOut)
            {
                _firstPerson = false;
                SetCameraBoomLength(CameraBoomMinimumDistance);
            }
            else
            {
                var lengthDelta = (zoomOut ? 1f : -1f) * 0.4f;
                var to = _cameraBoomTweener.To + lengthDelta;

                switch (to)
                {
                    case < CameraBoomMinimumDistance:
                        _firstPerson = true;
                        SetCameraBoomLength(0f);
                        return;
                    case > 6f:
                        return;
                    default:
                        _cameraBoomTweener.Tween(to, 100, EasingFunctions.Easing.OutQuart);
                        break;
                }
            }
        }
        */
    }
}