using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tivoli.Scripts.World
{
    // same implementation as vrchat mirror 
    
    [ExecuteInEditMode]
    public class WorldMirror : MonoBehaviour
    {
        public enum Resolution
        {
            Auto = 0,
            X256 = 256,
            X512 = 512,
            X1024 = 1024,
        }

        public enum AntialiasingSamples
        {
            X1 = 1,
            X2 = 2,
            X4 = 4,
            X8 = 8,
        }

        private class ReflectionData
        {
            public readonly RenderTexture[] Texture = new RenderTexture[2];
            public MaterialPropertyBlock PropertyBlock;
        }

        public bool disablePixelLights = true;
        public bool turnOffMirrorOcclusion = true;
        public LayerMask reflectLayers = -1;
        public Resolution resolution = Resolution.Auto;
        public AntialiasingSamples maximumAntialiasing = AntialiasingSamples.X4;

        private RenderTexture _temporaryRenderTexture;
        private readonly Dictionary<Camera, ReflectionData> _allReflectionData = new();

        private TivoliInputActions _inputActions;

        private Renderer _mirrorRenderer;
        private Camera _mirrorCamera;
        private Skybox _mirrorSkybox;

        private Matrix4x4 _parentTransform;
        private Quaternion _parentRotation;
        private int _playerLocalLayer;

        private bool _insideRendering;
        private readonly int[] _texturePropertyId = new int[2];

        private const int MAX_RESOLUTION = 2048;

        private void OnValidate()
        {
            _insideRendering = false;
            // check shader or material
        }

        private void Start()
        {
            _mirrorRenderer = GetComponent<Renderer>();
            if (_mirrorRenderer == null)
            {
                enabled = false;
            }
            else
            {
                var sharedMaterial = _mirrorRenderer.sharedMaterial;
                if (sharedMaterial == null)
                {
                    enabled = false;
                }
                else
                {
                    if (sharedMaterial.shader == Shader.Find("Tivoli/Mirror"))
                    {
                        _texturePropertyId[0] = Shader.PropertyToID("_ReflectionTex0");
                        _texturePropertyId[1] = Shader.PropertyToID("_ReflectionTex1");
                        _playerLocalLayer = LayerMask.NameToLayer("PlayerLocal");
                        Camera.onPostRender += CameraPostRender;
                    }
                }
            }

            if (enabled && _inputActions == null)
            {
                _inputActions = new TivoliInputActions();
                _inputActions.Enable();
                _inputActions.VRTracking.Enable();
                // _inputActions.VRTracking.CenterEyePosition.Enable();
                // _inputActions.VRTracking.CenterEyeRotation.Enable();
                _inputActions.VRTracking.LeftEyePosition.Enable();
                _inputActions.VRTracking.LeftEyeRotation.Enable();
                _inputActions.VRTracking.RightEyePosition.Enable();
                _inputActions.VRTracking.RightEyeRotation.Enable();
            }
        }

        private void OnWillRenderObject()
        {
            if (!enabled || !_mirrorRenderer || !_mirrorRenderer.enabled) return;

            var currentCamera = Camera.current;
            if (!currentCamera || currentCamera == _mirrorCamera || _insideRendering) return;

            _insideRendering = true;

            var reflectionData = GetReflectionData(currentCamera);

            var pixelLightCount = QualitySettings.pixelLightCount;
            if (disablePixelLights)
            {
                QualitySettings.pixelLightCount = 0;
            }

            UpdateCameraModes(currentCamera);
            UpdateParentTransform(currentCamera);

            if (currentCamera.stereoEnabled)
            {
                if (ShouldRenderLeftEye(currentCamera))
                {
                    RenderMirror(_temporaryRenderTexture, GetWorldEyePos(_inputActions.VRTracking.LeftEyePosition),
                        GetWorldEyeRot(_inputActions.VRTracking.LeftEyeRotation),
                        GetEyeProjectionMatrix(currentCamera, Camera.StereoscopicEye.Left)
                    );

                    Graphics.CopyTexture(_temporaryRenderTexture, 0, 0, 0, 0,
                        _temporaryRenderTexture.width, _temporaryRenderTexture.height,
                        reflectionData.Texture[0], 0, 0, 0, 0);
                }

                if (ShouldRenderRightEye(currentCamera))
                {
                    RenderMirror(_temporaryRenderTexture, GetWorldEyePos(_inputActions.VRTracking.RightEyePosition),
                        GetWorldEyeRot(_inputActions.VRTracking.RightEyeRotation),
                        GetEyeProjectionMatrix(currentCamera, Camera.StereoscopicEye.Right)
                    );

                    Graphics.CopyTexture(_temporaryRenderTexture, 0, 0, 0, 0,
                        _temporaryRenderTexture.width, _temporaryRenderTexture.height,
                        reflectionData.Texture[1], 0, 0, 0, 0);
                }
            }
            else if (ShouldRenderMonoscopic(currentCamera))
            {
                var currentCameraTransform = currentCamera.transform;
                RenderMirror(_temporaryRenderTexture, currentCameraTransform.position, currentCameraTransform.rotation,
                    currentCamera.projectionMatrix);

                Graphics.CopyTexture(_temporaryRenderTexture, 0, 0, 0, 0,
                    _temporaryRenderTexture.width, _temporaryRenderTexture.height,
                    reflectionData.Texture[0], 0, 0, 0, 0);
            }

            if (_temporaryRenderTexture)
            {
                RenderTexture.ReleaseTemporary(_temporaryRenderTexture);
                _temporaryRenderTexture = null;
            }

            _mirrorRenderer.SetPropertyBlock(reflectionData.PropertyBlock);

            if (disablePixelLights)
            {
                QualitySettings.pixelLightCount = pixelLightCount;
            }

            _insideRendering = false;
        }

        private void CameraPostRender(Camera currentCamera)
        {
            if (!_allReflectionData.ContainsKey(currentCamera)) return;

            var reflectionData = _allReflectionData[currentCamera];

            if (reflectionData.Texture[0] != null)
            {
                RenderTexture.ReleaseTemporary(reflectionData.Texture[0]);
                reflectionData.Texture[0] = null;
            }

            if (reflectionData.Texture[1] != null)
            {
                RenderTexture.ReleaseTemporary(reflectionData.Texture[0]);
                reflectionData.Texture[1] = null;
            }
        }

        private void OnDisable()
        {
            foreach (var reflectionData in _allReflectionData.Values)
            {
                if (reflectionData.Texture[0] != null)
                {
                    RenderTexture.ReleaseTemporary(reflectionData.Texture[0]);
                    reflectionData.Texture[0] = null;
                }

                if (reflectionData.Texture[1] != null)
                {
                    RenderTexture.ReleaseTemporary(reflectionData.Texture[0]);
                    reflectionData.Texture[1] = null;
                }
            }

            _allReflectionData.Clear();
        }

        private void OnDestroy()
        {
            if (_mirrorCamera == null) return;
            if (Application.isEditor)
            {
                DestroyImmediate(_mirrorCamera.gameObject);
            }
            else
            {
                Destroy(_mirrorCamera.gameObject);
            }

            _mirrorCamera = null;
            _mirrorSkybox = null;

            if (_inputActions != null)
            {
                _inputActions.Disable();
                _inputActions = null;
            }
        }

        private bool ShouldRenderLeftEye(Camera currentCamera)
        {
            var stereoTargetEye = currentCamera.stereoTargetEye;
            var flag = stereoTargetEye is StereoTargetEyeMask.Both or StereoTargetEyeMask.Left;
            if (!flag) return false;
            if (Vector3.Dot(GetWorldEyePos(_inputActions.VRTracking.LeftEyePosition) - transform.position,
                    GetNormalDirection()) <= 0.0)
            {
                flag = false;
            }

            return flag;
        }

        private bool ShouldRenderRightEye(Camera currentCamera)
        {
            var stereoTargetEye = currentCamera.stereoTargetEye;
            var flag = stereoTargetEye is StereoTargetEyeMask.Both or StereoTargetEyeMask.Right;
            if (!flag) return false;
            if (Vector3.Dot(GetWorldEyePos(_inputActions.VRTracking.RightEyePosition) - transform.position,
                    GetNormalDirection()) <= 0.0)
            {
                flag = false;
            }

            return flag;
        }

        private bool ShouldRenderMonoscopic(Component currentCamera) =>
            Vector3.Dot(currentCamera.transform.position - transform.position,
                GetNormalDirection()) > 0.0;

        private Vector3 GetWorldEyePos(InputAction action) =>
            _parentTransform.MultiplyPoint3x4(action.ReadValue<Vector3>());


        private Quaternion GetWorldEyeRot(InputAction action) =>
            _parentRotation * action.ReadValue<Quaternion>();

        private static Matrix4x4 GetEyeProjectionMatrix(Camera currentCamera, Camera.StereoscopicEye eye) =>
            currentCamera.GetStereoProjectionMatrix(eye);

        private Vector3 GetNormalDirection() => -transform.forward;

        private void RenderMirror(
            RenderTexture targetTexture,
            Vector3 cameraPosition,
            Quaternion cameraRotation,
            Matrix4x4 cameraProjectionMatrix
        )
        {
            _mirrorCamera.ResetWorldToCameraMatrix();
            _mirrorCamera.transform.position = cameraPosition;
            _mirrorCamera.transform.rotation = cameraRotation;
            _mirrorCamera.projectionMatrix = cameraProjectionMatrix;
            _mirrorCamera.cullingMask = -17 & ~(1 << _playerLocalLayer) & reflectLayers.value;
            _mirrorCamera.targetTexture = targetTexture;

            var position = transform.position;
            var normal = GetNormalDirection();
            _mirrorCamera.worldToCameraMatrix *= CalculateReflectionMatrix(Plane(position, normal));
            _mirrorCamera.projectionMatrix =
                _mirrorCamera.CalculateObliqueMatrix(CameraSpacePlane(_mirrorCamera, position, normal));

            _mirrorCamera.transform.position = GetPosition(_mirrorCamera.cameraToWorldMatrix);
            _mirrorCamera.transform.rotation = GetRotation(_mirrorCamera.cameraToWorldMatrix);

            var num = GL.invertCulling ? 1 : 0;
            GL.invertCulling = num == 0;
            _mirrorCamera.Render();
            GL.invertCulling = num != 0;
        }

        private void UpdateCameraModes(Camera src)
        {
            if (!_mirrorCamera)
            {
                var mirrorCameraGameObject =
                    new GameObject(
                        "Mirror Camera", typeof(Camera), typeof(Skybox), typeof(FlareLayer)
                    )
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                _mirrorSkybox = mirrorCameraGameObject.GetComponent<Skybox>();
                _mirrorCamera = mirrorCameraGameObject.GetComponent<Camera>();
                _mirrorCamera.enabled = false;
            }

            _mirrorCamera.clearFlags = src.clearFlags;
            _mirrorCamera.backgroundColor = src.backgroundColor;
            if (src.clearFlags == CameraClearFlags.Skybox)
            {
                var skybox = src.GetComponent<Skybox>();
                if (!skybox || !skybox.material)
                {
                    _mirrorSkybox.enabled = false;
                }
                else
                {
                    _mirrorSkybox.enabled = true;
                    _mirrorSkybox.material = skybox.material;
                }
            }

            _mirrorCamera.farClipPlane = src.farClipPlane;
            _mirrorCamera.nearClipPlane = src.nearClipPlane;
            _mirrorCamera.orthographic = src.orthographic;
            _mirrorCamera.aspect = src.aspect;
            _mirrorCamera.orthographicSize = src.orthographicSize;
            _mirrorCamera.useOcclusionCulling = !turnOffMirrorOcclusion;
            _mirrorCamera.allowMSAA = src.allowMSAA;
            if (src.stereoEnabled) return;
            _mirrorCamera.fieldOfView = src.fieldOfView;
        }

        private void UpdateParentTransform(Component currentCamera)
        {
            // if (currentCamera.transform.parent != null)
            // {
            //     _parentTransform = currentCamera.transform.parent.localToWorldMatrix;
            //     _parentRotation = currentCamera.transform.parent.rotation;
            // }
            // else
            // {
            var localRotation = _inputActions.VRTracking.CenterEyeRotation.ReadValue<Quaternion>();
            var matrix4X4 = Matrix4x4.TRS(_inputActions.VRTracking.CenterEyePosition.ReadValue<Vector3>(),
                localRotation, Vector3.one);
            // var localRotation = currentCamera.transform.rotation;
            // var matrix4X4 = Matrix4x4.TRS(currentCamera.transform.position, localRotation, Vector3.one);
            _parentTransform = currentCamera.transform.localToWorldMatrix * matrix4X4.inverse;
            _parentRotation = currentCamera.transform.rotation * Quaternion.Inverse(localRotation);
            // }
        }

        private ReflectionData GetReflectionData(Camera currentCamera)
        {
            if (!_allReflectionData.TryGetValue(currentCamera, out var reflectionData))
            {
                reflectionData = new ReflectionData()
                {
                    PropertyBlock = new MaterialPropertyBlock()
                };
                _allReflectionData[currentCamera] = reflectionData;
            }

            if (_temporaryRenderTexture) RenderTexture.ReleaseTemporary(_temporaryRenderTexture);
            if (reflectionData.Texture[0]) RenderTexture.ReleaseTemporary(reflectionData.Texture[0]);
            if (reflectionData.Texture[1]) RenderTexture.ReleaseTemporary(reflectionData.Texture[1]);

            var width = Mathf.Min(currentCamera.pixelWidth, MAX_RESOLUTION);
            var height = Mathf.Min(currentCamera.pixelWidth, MAX_RESOLUTION);
            if (resolution != Resolution.Auto)
            {
                width = (int) resolution;
                height = (int) resolution;
            }

            var antiAliasing = Mathf.Clamp(1, QualitySettings.antiAliasing, (int) maximumAntialiasing);

            _temporaryRenderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                RenderTextureReadWrite.Default, antiAliasing);

            if (currentCamera.stereoEnabled)
            {
                reflectionData.Texture[0] = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                    RenderTextureReadWrite.Default, 1);

                reflectionData.PropertyBlock.SetTexture(_texturePropertyId[0], (Texture) reflectionData.Texture[0]);

                reflectionData.Texture[1] = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                    RenderTextureReadWrite.Default, 1);

                reflectionData.PropertyBlock.SetTexture(_texturePropertyId[1], (Texture) reflectionData.Texture[1]);
            }
            else
            {
                reflectionData.Texture[0] = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGBHalf,
                    RenderTextureReadWrite.Default, 1);

                reflectionData.PropertyBlock.SetTexture(_texturePropertyId[0], reflectionData.Texture[0]);
            }

            return reflectionData;
        }

        private static Vector4 Plane(Vector3 position, Vector3 normal) =>
            new(normal.x, normal.y, normal.z, -Vector3.Dot(position, normal));

        private static Vector4 CameraSpacePlane(Camera camera, Vector3 position, Vector3 normal)
        {
            var worldToCameraMatrix = camera.worldToCameraMatrix;
            return Plane(worldToCameraMatrix.MultiplyPoint(position),
                worldToCameraMatrix.MultiplyVector(normal).normalized);
        }

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

        private static Vector3 GetPosition(Matrix4x4 matrix)
        {
            double m03 = (double) matrix.m03;
            float m13 = matrix.m13;
            float m23 = matrix.m23;
            double y = (double) m13;
            double z = (double) m23;
            return new Vector3((float) m03, (float) y, (float) z);
        }
    }
}