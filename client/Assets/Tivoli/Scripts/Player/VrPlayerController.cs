using Tivoli.Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tivoli.Scripts.Player
{
    [DefaultExecutionOrder(-30000)]
    public class VrPlayerController : MonoBehaviour
    {
        private Camera _mainCamera;
        private Transform _mainCameraTransform;

        public Animator animator;

        private Rigidbody _rigidbody;

        private TivoliInputActions _inputActions;

        private Vector3 _lastRigidbodyXZCenterEyePosition;

        private bool _snapTurnHoldingDown;
        private const float TurnDeadzone = 0.2f;
        private const float TurnDegrees = 30f;

        public VrPlayerIkController ikController;

        private void Awake()
        {
            _mainCamera = DependencyManager.Instance.UIManager.GetMainCamera();
            _mainCameraTransform = _mainCamera.transform;

            _rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _mainCamera.transform.SetParent(transform);

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

            animator.GetBoneTransform(HumanBodyBones.Head).localScale = Vector3.zero;

            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable()
        {
            _mainCamera.transform.SetParent(null);

            _inputActions.Disable();

            Application.onBeforeRender -= OnBeforeRender;
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

        public class IkData
        {
            public Vector3 HeadPosition;
            public Quaternion HeadRotation;
            public Vector3 LeftHandPosition;
            public Quaternion LeftHandRotation;
            public Vector3 RightHandPosition;
            public Quaternion RightHandRotation;
        }

        // public Vector3 leftHandRotateOffset = new(45, 60, 240);
        // public Vector3 rightHandRotateOffset = new(-45, 120, 240);
        public Vector3 leftHandRotateOffset = new(90, 0, 0);
        public Vector3 rightHandRotateOffset = new(90, 0, 0);

        public IkData GetIkData()
        {
            var playerPosition = transform.position;
            var playerY = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            var leftHandPosition =
                playerY * (_inputActions.VRTracking.LeftHandPosition.ReadValue<Vector3>() -
                           _lastRigidbodyXZCenterEyePosition) + playerPosition;

            var leftHandRotation = playerY * _inputActions.VRTracking.LeftHandRotation.ReadValue<Quaternion>() *
                                   Quaternion.Euler(leftHandRotateOffset.x, leftHandRotateOffset.y,
                                       leftHandRotateOffset.z);

            var rightHandPosition =
                playerY * (_inputActions.VRTracking.RightHandPosition.ReadValue<Vector3>() -
                           _lastRigidbodyXZCenterEyePosition) + playerPosition;

            var rightHandRotation = playerY * _inputActions.VRTracking.RightHandRotation.ReadValue<Quaternion>() *
                                    Quaternion.Euler(rightHandRotateOffset.x, rightHandRotateOffset.y,
                                        rightHandRotateOffset.z);

            return new IkData
            {
                HeadPosition = _mainCamera.transform.position,
                HeadRotation = _mainCamera.transform.rotation,
                LeftHandPosition = leftHandPosition,
                LeftHandRotation = leftHandRotation,
                RightHandPosition = rightHandPosition,
                RightHandRotation = rightHandRotation,
            };
        }

        private void LateUpdate()
        {
            // update ik

            var ikData = GetIkData();
            ikController.UpdateWithIkData(ikData);
        }
    }
}