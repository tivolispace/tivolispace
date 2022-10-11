using System.Collections.Generic;
using System.Linq;
using Tivoli.Scripts.Player.Hifi;
using UnityEngine;

namespace Tivoli.Scripts.Player
{
    public class VrPlayerIkControllerHifi : MonoBehaviour
    {
        public Animator animator;

        private TivoliInputActions _inputActions;

        private Vector3 _tposeHipsPosition = Vector3.zero;
        private readonly Dictionary<HumanBodyBones, SkeletonBone> _tposeBones = new();

        // private readonly Dictionary<HumanBodyBones, Quaternion> _boneRotationOffsets = new();

        public Transform testHead;
        public Transform testHips;

        private MyAvatar _hifiMyAvatar;

        private void Awake()
        {
            _inputActions = new TivoliInputActions();
            _inputActions.Enable();
            _inputActions.VRTracking.Enable();
            _inputActions.VRTracking.CenterEyePosition.Enable();

            // get t-pose bones

            // below is in local space. if we do GetBoneTransform fast enough, maybe it'll be okay for now
            // TODO: look into this

            // var nameToBoneDictionary = new Dictionary<string, HumanBodyBones>();
            // for (var i = 0; i < (int) HumanBodyBones.LastBone; i++)
            // {
            //     var humanBodyBone = (HumanBodyBones) i;
            //     var boneTransform = animator.GetBoneTransform(humanBodyBone);
            //     if (boneTransform == null) continue;
            //     nameToBoneDictionary[boneTransform.name] = humanBodyBone;
            // }
            //
            // var skeletonBones = animator.avatar.humanDescription.skeleton;
            // foreach (var skeletonBone in skeletonBones)
            // {
            //     if (nameToBoneDictionary.TryGetValue(skeletonBone.name, out var humanBodyBone))
            //     {
            //         _tposeBones[humanBodyBone] = skeletonBone;
            //     }
            // }

            for (var i = 0; i < (int) HumanBodyBones.LastBone; i++)
            {
                var humanBodyBone = (HumanBodyBones) i;
                var boneTransform = animator.GetBoneTransform(humanBodyBone);
                if (boneTransform == null) continue;
                _tposeBones[humanBodyBone] = new SkeletonBone
                {
                    name = boneTransform.name,
                    position = boneTransform.position,
                    rotation = boneTransform.rotation,
                    scale = Vector3.one
                };
            }

            _tposeHipsPosition = _tposeBones[HumanBodyBones.Hips].position;

            // negate all bones with hips position because hifi uses hips as 0,0,0

            if (_tposeHipsPosition != Vector3.zero)
            {
                foreach (var boneKey in _tposeBones.Keys.ToArray())
                {
                    var skeletonBone = _tposeBones[boneKey];
                    skeletonBone.position -= _tposeHipsPosition;
                    _tposeBones[boneKey] = skeletonBone;
                }
            }

            // hips need to be euler 0,0,0 so lets make a dictionary of what bone rotations should be

            // var boneRotationResets = new[]
            // {
            //     (HumanBodyBones.Hips, new Vector3(0f, 0f, 0f)),
            //     (HumanBodyBones.Head, new Vector3(0f, 0f, 0f)),
            // };
            //
            // foreach (var (bone, eulerReset) in boneRotationResets)
            // {
            //     var tposeBone = _tposeBones[bone];
            //     var rotReset = Quaternion.Euler(eulerReset);
            //     _boneRotationOffsets[bone] = Quaternion.Inverse(tposeBone.rotation);
            //     tposeBone.rotation = rotReset;
            //     _tposeBones[bone] = tposeBone;
            // }

            _hifiMyAvatar = new MyAvatar
            {
                // avatar bone position
                GetAvatarBonePos = (bone) =>
                    animator.GetBoneTransform(bone).position - _tposeHipsPosition - transform.position,

                GetAvatarDefaultBonePos = bone => _tposeBones[bone].position,

                // avatar bone rotation
                GetAvatarBoneRot = bone =>
                {
                    var rotation = animator.GetBoneTransform(bone).rotation;
                    // if (_boneRotationOffsets.TryGetValue(bone, out var offset))
                    // {
                    //     rotation = offset * rotation;
                    // }
                    return rotation;
                },

                GetAvatarDefaultBoneRot = bone => _tposeBones[bone].rotation,

                // user eyes
                GetUserEyePosition = () => testHead.transform.position,
                GetUserEyeRotation = () => testHead.transform.rotation
            };
        }

        private void OnDestroy()
        {
        }

        public void Update()
        {
            var hipsMat = _hifiMyAvatar.DeriveBodyUsingCgModel();
            testHips.position = hipsMat.GetPosition() + _tposeHipsPosition;
            testHips.rotation = hipsMat.rotation;
        }
    }
}