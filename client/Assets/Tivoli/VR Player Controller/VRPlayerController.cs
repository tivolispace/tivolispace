using System;
using UnityEngine;

namespace Tivoli.VR_Player_Controller
{
    [DefaultExecutionOrder(-30000)]
    public class VRPlayerController : MonoBehaviour
    {
        public Camera _mainCamera;
        private Transform _mainCameraTransform;

        public Animator _animator;

        private Rigidbody _rigidbody;

        private TivoliInputActions _inputActions;

        private Vector3 _lastRigidbodyXZCenterEyePosition;

        private bool _snapTurnHoldingDown;
        private const float TurnDeadzone = 0.2f;
        private const float TurnDegrees = 30f;

        void Awake()
        {
            _mainCameraTransform = _mainCamera.transform;

            _rigidbody = gameObject.GetComponent<Rigidbody>();

            _inputActions = new TivoliInputActions();

            _inputActions.Player.Enable();
            _inputActions.Player.Move.Enable();
            _inputActions.Player.Turn.Enable();

            _inputActions.VRTracking.Enable();
            _inputActions.VRTracking.CenterEyePosition.Enable();
            _inputActions.VRTracking.CenterEyeRotation.Enable();
            _inputActions.VRTracking.LeftHandPosition.Enable();
            _inputActions.VRTracking.LeftHandRotation.Enable();
            _inputActions.VRTracking.RightHandPosition.Enable();
            _inputActions.VRTracking.RightHandRotation.Enable();

            _animator.GetBoneTransform(HumanBodyBones.Head).localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            var playerPosition = transform.position;
            var playerY = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            var leftHandPosition =
                playerY * (_inputActions.VRTracking.LeftHandPosition.ReadValue<Vector3>() -
                           _lastRigidbodyXZCenterEyePosition) + playerPosition;

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);

            var leftHandRotation = playerY * _inputActions.VRTracking.LeftHandRotation.ReadValue<Quaternion>() *
                                   Quaternion.Euler(0, 0, 90);


            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRotation);

            var rightHandPosition =
                playerY * (_inputActions.VRTracking.RightHandPosition.ReadValue<Vector3>() -
                           _lastRigidbodyXZCenterEyePosition) + playerPosition;

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition);

            var rightHandRotation = playerY * _inputActions.VRTracking.RightHandRotation.ReadValue<Quaternion>() *
                                    Quaternion.Euler(0, 0, -90);

            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotation);
        }

        private void TrackingUpdate()
        {
            _mainCameraTransform.localRotation = _inputActions.VRTracking.CenterEyeRotation.ReadValue<Quaternion>();

            // compensating for slower rigid body update loop
            // test this by setting time step to 0.1 or higher
            var centerEyePosition = _inputActions.VRTracking.CenterEyePosition.ReadValue<Vector3>();
            var xzCenterEyePosition = new Vector3(centerEyePosition.x, 0, centerEyePosition.z);
            _mainCameraTransform.localPosition =
                Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(0, centerEyePosition.y, 0) + (
                    xzCenterEyePosition - _lastRigidbodyXZCenterEyePosition
                );

            // var centerEyePosition = _inputActions.VRTracking.CenterEyePosition.ReadValue<Vector3>();
            // _mainCameraTransform.localPosition = new Vector3(0, centerEyePosition.y, 0);
        }

        private void Update()
        {
            TrackingUpdate();
        }

        [BeforeRenderOrder(-30000)]
        private void OnBeforeRender()
        {
            TrackingUpdate();
        }

        private void FixedUpdate()
        {
            // snap turning

            var turnValue = _inputActions.Player.Turn.ReadValue<Vector2>().x;
            var turnDir = turnValue switch
            {
                < -TurnDeadzone => -1,
                > TurnDeadzone => 1,
                _ => 0
            };

            if (_snapTurnHoldingDown)
            {
                if (turnDir == 0)
                {
                    _snapTurnHoldingDown = false;
                }
            }
            else
            {
                if (turnDir != 0)
                {
                    _snapTurnHoldingDown = true;

                    var degrees = turnDir * TurnDegrees;

                    _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, degrees, 0));
                }
            }

            // moving

            var centerEyePosition = _inputActions.VRTracking.CenterEyePosition.ReadValue<Vector3>();
            var centerEyeRotation = _inputActions.VRTracking.CenterEyeRotation.ReadValue<Quaternion>();
            var cameraY = centerEyeRotation.eulerAngles.y;

            var moveXy = _inputActions.Player.Move.ReadValue<Vector2>();
            var playerY = transform.eulerAngles.y;

            // moving input

            var xzMoveOffset = Quaternion.Euler(0, playerY + cameraY, 0) * new Vector3(moveXy.x, 0, moveXy.y) * 0.05f;

            // moving head

            var xzCenterEyePosition = new Vector3(centerEyePosition.x, 0, centerEyePosition.z);
            var xzCameraOffset = Quaternion.Euler(0, playerY, 0) *
                                 (xzCenterEyePosition - _lastRigidbodyXZCenterEyePosition);

            _lastRigidbodyXZCenterEyePosition = xzCenterEyePosition;

            // moving apply

            _rigidbody.MovePosition(transform.position + xzCameraOffset + xzMoveOffset);

            // tracking

            TrackingUpdate();
        }
    }
}