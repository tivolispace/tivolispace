using System;
using System.Collections.Generic;
using System.Linq;
using DitzelGames.FastIK;
using Unity.VisualScripting;
using UnityEngine;

namespace Tivoli.VR_Player_Controller
{
    public class VRIKController : MonoBehaviour
    {
        public Animator animator;

        private float _floorToHipsToHeadRatio;

        private GameObject _headTarget;
        private GameObject _headPole;
        private FastIKFabric _headIk;

        private GameObject _leftHandTarget;
        private FastIKFabric _leftHandIk;

        private GameObject _rightHandTarget;
        private FastIKFabric _rightHandIk;

        private GameObject _leftFootTarget;
        private GameObject _leftFootPole;
        private FastIKFabric _leftFootIk;

        private GameObject _rightFootTarget;
        private GameObject _rightFootPole;
        private FastIKFabric _rightFootIk;

        private int CountBonesIfExists(IEnumerable<HumanBodyBones> bones)
        {
            return bones.Select(bone => animator.GetBoneTransform(bone))
                .Count(boneTransform => boneTransform != null);
        }

        private void Awake()
        {
            // get ratio from floor to hips to head

            var headDistanceFromFloor =
                animator.GetBoneTransform(HumanBodyBones.Head).position.y - transform.position.y;
            var hipsDistanceFromFloor =
                animator.GetBoneTransform(HumanBodyBones.Hips).position.y - transform.position.y;

            _floorToHipsToHeadRatio = hipsDistanceFromFloor / headDistanceFromFloor;

            // head

            var headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            
            _headTarget = new GameObject("IK Head Target");
            _headTarget.transform.SetParent(transform);
            _headTarget.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            
            _headPole = new GameObject("IK Head Pole");
            _headPole.transform.SetParent(_headTarget.transform);
            _headPole.transform.localPosition = new Vector3(0f, 0f, -0.5f);

            _headIk = headTransform.AddComponent<FastIKFabric>();
            _headIk.Target = _headTarget.transform;
            _headIk.Pole = _headPole.transform;
            _headIk.ChainLength = CountBonesIfExists(new[]
            {
                HumanBodyBones.Neck, HumanBodyBones.UpperChest, HumanBodyBones.Chest,
                HumanBodyBones.Spine, HumanBodyBones.Hips
            });

            // left hand

            var leftHandTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            _leftHandTarget = new GameObject("IK Left Hand Target");
            _leftHandTarget.transform.SetParent(transform);
            _leftHandTarget.transform.localPosition = new Vector3(-0.2f, 1f, 0.1f);

            _leftHandIk = leftHandTransform.AddComponent<FastIKFabric>();
            _leftHandIk.Target = _leftHandTarget.transform;
            _leftHandIk.ChainLength = CountBonesIfExists(new[]
            {
                HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftUpperArm,
                HumanBodyBones.LeftShoulder
            });

            // right hand

            var rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
            _rightHandTarget = new GameObject("IK Right Hand Target");
            _rightHandTarget.transform.SetParent(transform);
            _rightHandTarget.transform.localPosition = new Vector3(0.2f, 1f, 0.1f);

            _rightHandIk = rightHandTransform.AddComponent<FastIKFabric>();
            _rightHandIk.Target = _rightHandTarget.transform;
            _rightHandIk.ChainLength = CountBonesIfExists(new[]
            {
                HumanBodyBones.RightLowerArm, HumanBodyBones.RightUpperArm,
                HumanBodyBones.RightShoulder
            });

            // left foot

            var leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            var leftToeTransform = animator.GetBoneTransform(HumanBodyBones.LeftToes);
            var distanceFromLeftToeToFoot = leftFootTransform.position.y - leftToeTransform.position.y;
            
            _leftFootTarget = new GameObject("IK Left Foot Target");
            _leftFootTarget.transform.SetParent(transform);
            _leftFootTarget.transform.localPosition = new Vector3(-0.05f, distanceFromLeftToeToFoot, 0f);
            
            _leftFootPole = new GameObject("IK Left Foot Pole");
            _leftFootPole.transform.SetParent(_leftFootTarget.transform);
            _leftFootPole.transform.localPosition = new Vector3(0f, 0f, 0.5f);

            _leftFootIk = leftFootTransform.AddComponent<FastIKFabric>();
            _leftFootIk.Target = _leftFootTarget.transform;
            _leftFootIk.Pole = _leftFootPole.transform;
            _leftFootIk.ChainLength = CountBonesIfExists(new[]
            {
                HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftUpperArm
            });

            // right foot

            var rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            var rightToeTransform = animator.GetBoneTransform(HumanBodyBones.RightToes);
            var distanceFromRightToeToFoot = rightFootTransform.position.y - rightToeTransform.position.y;
            
            _rightFootTarget = new GameObject("IK Right Foot Target");
            _rightFootTarget.transform.SetParent(transform);
            _rightFootTarget.transform.localPosition = new Vector3(0.05f, distanceFromRightToeToFoot, 0f);
            
            _rightFootPole = new GameObject("IK Left Foot Pole");
            _rightFootPole.transform.SetParent(_rightFootTarget.transform);
            _rightFootPole.transform.localPosition = new Vector3(0f, 0f, 0.5f);

            _rightFootIk = rightFootTransform.AddComponent<FastIKFabric>();
            _rightFootIk.Target = _rightFootTarget.transform;
            _rightFootIk.Pole = _rightFootPole.transform;
            _rightFootIk.ChainLength = CountBonesIfExists(new[]
            {
                HumanBodyBones.RightLowerLeg, HumanBodyBones.RightUpperArm
            });
        }

        private void OnDestroy()
        {
            Destroy(_headTarget);
            Destroy(_leftHandTarget);
            Destroy(_rightHandTarget);
            Destroy(_leftFootTarget);
            Destroy(_rightFootTarget);
        }

        private void LateUpdate()
        {
            // calculate hips position with ratio

            // var headFromFloor = (animator.GetBoneTransform(HumanBodyBones.Head).position - transform.position).y;
            var headFromFloor = (_headTarget.transform.position - transform.position).y;

            var hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            hipsTransform.position = new Vector3(hipsTransform.position.x,
                headFromFloor * _floorToHipsToHeadRatio + transform.position.y,
                hipsTransform.position.z);
        }

        public void UpdateHead(Vector3 position, Quaternion rotation, float distanceToEyes)
        {
            _headTarget.transform.position = position;
            var localPosition = _headTarget.transform.localPosition;
            localPosition.z -= distanceToEyes;
            _headTarget.transform.localPosition = localPosition;
            _headTarget.transform.rotation = rotation;
            
        }

        public void UpdateLeftHand(Vector3 position, Quaternion rotation)
        {
            _leftHandTarget.transform.position = position;
            _leftHandTarget.transform.rotation = rotation;
        }

        public void UpdateRightHand(Vector3 position, Quaternion rotation)
        {
            _rightHandTarget.transform.position = position;
            _rightHandTarget.transform.rotation = rotation;
        }
    }
}