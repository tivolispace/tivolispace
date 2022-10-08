using System;
using Tivoli.Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tivoli.Scripts.World
{
    public class WorldMirror : MonoBehaviour
    {
        private Camera _mainCamera;

        private TivoliInputActions _inputActions;

        private bool _enabled;

        private RenderTexture _leftEyeTexture;
        private RenderTexture _rightEyeTexture;

        private GameObject _mirrorCameraGameObject;
        private Camera _mirrorCamera;
        private Skybox _mirrorSkybox;

        private Matrix4x4 _parentTransform;
        private Quaternion _parentRotation;

        private void Awake()
        {
            _inputActions = new TivoliInputActions();
            _inputActions.Enable();
            _inputActions.VRTracking.Enable();
            _inputActions.VRTracking.CenterEyePosition.Enable();
            _inputActions.VRTracking.CenterEyeRotation.Enable();
            _inputActions.VRTracking.LeftEyePosition.Enable();
            _inputActions.VRTracking.LeftEyeRotation.Enable();
            _inputActions.VRTracking.RightEyePosition.Enable();
            _inputActions.VRTracking.RightEyeRotation.Enable();

            _mainCamera = DependencyManager.Instance.UIManager.GetMainCamera();

            var width = Mathf.Min(_mainCamera.pixelWidth, 2048);
            var height = Mathf.Min(_mainCamera.pixelHeight, 2048);

            _leftEyeTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                RenderTextureReadWrite.Default, 1);

            GetComponent<MeshRenderer>().material.SetTexture(Shader.PropertyToID("_ReflectionTex0"), _leftEyeTexture);

            _rightEyeTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                RenderTextureReadWrite.Default, 1);

            GetComponent<MeshRenderer>().material.SetTexture(Shader.PropertyToID("_ReflectionTex1"), _rightEyeTexture);

            _mirrorCameraGameObject =
                new GameObject("Mirror Camera", typeof(Camera), typeof(Skybox), typeof(FlareLayer))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

            _mirrorCamera = _mirrorCameraGameObject.GetComponent<Camera>();
            _mirrorCamera.enabled = false;

            _mirrorSkybox = _mirrorCameraGameObject.GetComponent<Skybox>();

            _enabled = true;
        }

        private void OnDestroy()
        {
            _enabled = false;

            Destroy(_mirrorCameraGameObject);

            RenderTexture.ReleaseTemporary(_leftEyeTexture);
            RenderTexture.ReleaseTemporary(_rightEyeTexture);

            _inputActions.Disable();
        }

        private Vector3 GetNormalDirection()
        {
            return -transform.forward;
        }

        private static Vector4 Plane(Vector3 position, Vector3 normal) =>
            new(normal.x, normal.y, normal.z, -Vector3.Dot(position, normal));

        private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
        {
            Matrix4x4 m;
            m.m00 = (float) (1.0 - 2.0 * plane[0] * plane[0]);
            m.m01 = -2f * plane[0] * plane[1];
            m.m02 = -2f * plane[0] * plane[2];
            m.m03 = -2f * plane[3] * plane[0];
            m.m10 = -2f * plane[1] * plane[0];
            m.m11 = (float) (1.0 - 2.0 * plane[1] * plane[1]);
            m.m12 = -2f * plane[1] * plane[2];
            m.m13 = -2f * plane[3] * plane[1];
            m.m20 = -2f * plane[2] * plane[0];
            m.m21 = -2f * plane[2] * plane[1];
            m.m22 = (float) (1.0 - 2.0 * plane[2] * plane[2]);
            m.m23 = -2f * plane[3] * plane[2];
            m.m30 = 0.0f;
            m.m31 = 0.0f;
            m.m32 = 0.0f;
            m.m33 = 1f;
            return m;
        }

        private static Vector4 CameraSpacePlane(Camera camera, Vector3 position, Vector3 normal)
        {
            var worldToCameraMatrix = camera.worldToCameraMatrix;
            return Plane(worldToCameraMatrix.MultiplyPoint(position),
                worldToCameraMatrix.MultiplyVector(normal).normalized);
        }

        private static Vector3 GetPosition(Matrix4x4 matrix)
        {
            double m03 = (double) matrix.m03;
            float m13 = matrix.m13;
            float m23 = matrix.m23;
            double y = (double) m13;
            double z = (double) m23;
            return new Vector3((float) m03, (float) y, (float) z);
        }

        private static float CopySign(float sizeValue, float signValue) => Mathf.Sign(signValue) * Mathf.Abs(sizeValue);

        private static Quaternion GetRotation(Matrix4x4 matrix)
        {
            var rotation = new Quaternion
            {
                w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f,
                x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f,
                y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f,
                z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f
            };
            rotation.x = CopySign(rotation.x, matrix.m21 - matrix.m12);
            rotation.y = CopySign(rotation.y, matrix.m02 - matrix.m20);
            rotation.z = CopySign(rotation.z, matrix.m10 - matrix.m01);
            return rotation;
        }

        private void RenderEye(
            Vector3 eyePosition,
            Quaternion eyeRotation,
            Matrix4x4 eyeProjectionMatrix,
            RenderTexture targetTexture
        )
        {
            var planePosition = transform.position;
            var planeNormal = GetNormalDirection();

            _mirrorCamera.ResetWorldToCameraMatrix();
            _mirrorCamera.transform.position = eyePosition;
            _mirrorCamera.transform.rotation = eyeRotation;
            _mirrorCamera.projectionMatrix = eyeProjectionMatrix;
            _mirrorCamera.targetTexture = targetTexture;
            _mirrorCamera.worldToCameraMatrix *= CalculateReflectionMatrix(Plane(planePosition, planeNormal));
            _mirrorCamera.projectionMatrix =
                _mirrorCamera.CalculateObliqueMatrix(CameraSpacePlane(_mirrorCamera, planePosition, planeNormal));
            _mirrorCamera.transform.position = GetPosition(_mirrorCamera.cameraToWorldMatrix);
            _mirrorCamera.transform.rotation = GetRotation(_mirrorCamera.cameraToWorldMatrix);
            var num = GL.invertCulling ? 1 : 0;
            GL.invertCulling = num == 0;
            _mirrorCamera.Render();
            GL.invertCulling = num != 0;
        }

        private Vector3 GetWorldEyePosition(InputAction action)
        {
            return _parentTransform.MultiplyPoint3x4(action.ReadValue<Vector3>());
        }

        private Quaternion GetWorldEyeRotation(InputAction action)
        {
            return _parentRotation * action.ReadValue<Quaternion>();
        }

        private Matrix4x4 GetEyeProjectionMatrix(Camera.StereoscopicEye eye)
        {
            return _mainCamera.GetStereoProjectionMatrix(eye);
        }

        private void UpdateMirrorCamera()
        {
            _mirrorCamera.clearFlags = _mainCamera.clearFlags;
            _mirrorCamera.backgroundColor = _mainCamera.backgroundColor;
            if (_mainCamera.clearFlags == CameraClearFlags.Skybox)
            {
                var skybox = _mainCamera.GetComponent<Skybox>();
                if (skybox != null && skybox.material != null)
                {
                    _mirrorSkybox.enabled = true;
                    _mirrorSkybox.material = skybox.material;
                }
                else
                {
                    _mirrorSkybox.enabled = false;
                }
            }

            _mirrorCamera.farClipPlane = _mainCamera.farClipPlane;
            _mirrorCamera.nearClipPlane = _mainCamera.nearClipPlane;
            _mirrorCamera.orthographic = _mainCamera.orthographic;
            _mirrorCamera.aspect = _mainCamera.aspect;
            _mirrorCamera.orthographicSize = _mainCamera.orthographicSize;
            _mirrorCamera.useOcclusionCulling = _mainCamera.useOcclusionCulling;
            _mirrorCamera.allowMSAA = _mainCamera.allowMSAA;

            // if (notStereo)
            // _mirrorCamera.fieldOfView = _mainCamera.fieldOfView;
        }

        private void UpdateParentTransform()
        {
            // if (_mainCamera.transform.parent != null)
            // {
            //     _parentTransform = _mainCamera.transform.parent.localToWorldMatrix;
            //     _parentRotation = _mainCamera.transform.parent.rotation;
            // }
            // else
            // {
            var localRotation = _inputActions.VRTracking.CenterEyeRotation.ReadValue<Quaternion>();
            var matrix4x4 = Matrix4x4.TRS(_inputActions.VRTracking.CenterEyePosition.ReadValue<Vector3>(),
                localRotation, Vector3.one);
            _parentTransform = _mainCamera.transform.localToWorldMatrix * matrix4x4.inverse;
            _parentRotation = _mainCamera.transform.rotation * Quaternion.Inverse(localRotation);
            // }
        }

        private bool ShouldRenderLeftEye()
        {
            var stereoTargetEye = _mainCamera.stereoTargetEye;
            var flag = stereoTargetEye is StereoTargetEyeMask.Both or StereoTargetEyeMask.Left;
            if (!flag) return false;
            if (Vector3.Dot(GetWorldEyePosition(_inputActions.VRTracking.LeftEyePosition) - transform.position,
                    GetNormalDirection()) <= 0.0)
            {
                flag = false;
            }

            return flag;
        }


        private bool ShouldRenderRightEye()
        {
            var stereoTargetEye = _mainCamera.stereoTargetEye;
            var flag = stereoTargetEye is StereoTargetEyeMask.Both or StereoTargetEyeMask.Right;
            if (!flag) return false;
            if (Vector3.Dot(GetWorldEyePosition(_inputActions.VRTracking.RightEyePosition) - transform.position,
                    GetNormalDirection()) <= 0.0)
            {
                flag = false;
            }


            return flag;
        }

        private void OnWillRenderObject()
        {
            if (!_enabled) return;

            var current = Camera.current;
            if (current == _mirrorCamera) return;

            UpdateMirrorCamera();
            UpdateParentTransform();

            if (ShouldRenderLeftEye())
            {
                RenderEye(
                    GetWorldEyePosition(_inputActions.VRTracking.LeftEyePosition),
                    GetWorldEyeRotation(_inputActions.VRTracking.LeftEyeRotation),
                    GetEyeProjectionMatrix(Camera.StereoscopicEye.Left),
                    _leftEyeTexture
                );
            }

            if (ShouldRenderRightEye())
            {
                RenderEye(
                    GetWorldEyePosition(_inputActions.VRTracking.RightEyePosition),
                    GetWorldEyeRotation(_inputActions.VRTracking.RightEyeRotation),
                    GetEyeProjectionMatrix(Camera.StereoscopicEye.Right),
                    _rightEyeTexture
                );
            }
        }
    }
}