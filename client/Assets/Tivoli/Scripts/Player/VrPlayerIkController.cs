using System.Linq;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tivoli.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class VrPlayerIkController : MonoBehaviour
    {
        [FormerlySerializedAs("eyePosition")] public Vector3 eyePositionOffset;

        private Animator _animator;
        private VRIK _vrik;

        private GameObject _headTarget;
        private GameObject _headTargetOffset;

        private GameObject _leftHandTargetOffset;
        private GameObject _leftHandTarget;

        private GameObject _rightHandTargetOffset;
        private GameObject _rightHandTarget;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _vrik = gameObject.AddComponent<VRIK>();
            _vrik.AutoDetectReferences();

            _vrik.solver.plantFeet = false;
            _vrik.solver.locomotion.mode = IKSolverVR.Locomotion.Mode.Animated;
            _vrik.solver.locomotion.footDistance = 0.1f;

            // head
            var headBone = Utils.AnimatorUtils.GetAnimatorSpaceTposeBone(_animator, HumanBodyBones.Head);

            _headTarget = new GameObject("IK Head Target");
            _headTarget.transform.SetParent(transform.parent);
            _headTarget.transform.localPosition = new Vector3(0f, 1.3f, 0f);

            _headTargetOffset = new GameObject("IK Head Target Offset");
            _headTargetOffset.transform.SetParent(_headTarget.transform);
            _headTargetOffset.transform.localPosition =
                Vector3.Scale(headBone.position, _animator.transform.localScale) - eyePositionOffset;
            _headTargetOffset.transform.localRotation = headBone.rotation;

            _vrik.solver.spine.headTarget = _headTargetOffset.transform;

            // left hand
            var leftHandBone = Utils.AnimatorUtils.GetAnimatorSpaceTposeBone(_animator, HumanBodyBones.Head);

            _leftHandTarget = new GameObject("IK Left Hand Target");
            _leftHandTarget.transform.SetParent(transform.parent);
            _leftHandTarget.transform.localPosition = new Vector3(-0.2f, 1f, 0.1f);

            _leftHandTargetOffset = new GameObject("IK Left Hand Target Offset");
            _leftHandTargetOffset.transform.SetParent(_leftHandTarget.transform);
            _leftHandTargetOffset.transform.localPosition = Vector3.zero;
            _leftHandTargetOffset.transform.localRotation = leftHandBone.rotation * Quaternion.Euler(0f, 180f, 0f);

            _vrik.solver.leftArm.target = _leftHandTargetOffset.transform;

            // right hand
            var rightHandBone = Utils.AnimatorUtils.GetAnimatorSpaceTposeBone(_animator, HumanBodyBones.Head);

            _rightHandTarget = new GameObject("IK Right Hand Target");
            _rightHandTarget.transform.SetParent(transform.parent);
            _rightHandTarget.transform.localPosition = new Vector3(0.2f, 1f, 0.1f);

            _rightHandTargetOffset = new GameObject("IK Right Hand Target Offset");
            _rightHandTargetOffset.transform.SetParent(_rightHandTarget.transform);
            _rightHandTargetOffset.transform.localPosition = Vector3.zero;
            _rightHandTargetOffset.transform.localRotation = rightHandBone.rotation * Quaternion.Euler(0f, 180f, 0f);

            _vrik.solver.rightArm.target = _rightHandTargetOffset.transform;

            // knees
            // TODO: figure this out
            _vrik.solver.leftLeg.swivelOffset = -90f;
            _vrik.solver.rightLeg.swivelOffset = 90f;
        }

        private void OnDestroy()
        {
            Destroy(_headTarget);
            Destroy(_leftHandTarget);
            Destroy(_rightHandTarget);
        }

        public void UpdateWithIkData(IkData ikData)
        {
            // has offset for eye position so this is correct
            _headTarget.transform.localPosition = new Vector3(0, ikData.LocalEyeHeight, 0);
            _headTarget.transform.rotation = ikData.EyeRotation;

            _leftHandTarget.transform.localPosition = ikData.LocalLeftHandPosition;
            _leftHandTarget.transform.localRotation = ikData.LocalLeftHandRotation;

            _rightHandTarget.transform.localPosition = ikData.LocalRightHandPosition;
            _rightHandTarget.transform.localRotation = ikData.LocalRightHandRotation;
        }
    }
}