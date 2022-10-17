using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tivoli.Scripts.Utils
{
    public static class AnimatorUtils
    {
        public static SkeletonBone GetAnimatorSpaceTposeBone(Animator animator, HumanBodyBones humanBodyBone)
        {
            // get bone to root names (root not included)

            var boneToRootNames = new List<string>();

            var currentBoneTransform = animator.GetBoneTransform(humanBodyBone);
            boneToRootNames.Add(currentBoneTransform.name);

            while (true)
            {
                currentBoneTransform = currentBoneTransform.parent;
                if (currentBoneTransform == animator.transform) break;
                boneToRootNames.Add(currentBoneTransform.name);
            }

            // now get each value in skeleton

            var skeleton = animator.avatar.humanDescription.skeleton;

            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            var scale = Vector3.one;

            foreach (var boneName in boneToRootNames)
            {
                SkeletonBone skeletonBone;
                try
                {
                    skeletonBone = skeleton.First(bone => bone.name == boneName);
                }
                catch (Exception _)
                {
                    continue;
                }

                position = skeletonBone.rotation * position + skeletonBone.position;
                rotation *= skeletonBone.rotation;
                scale = Vector3.Scale(scale, skeletonBone.scale);
            }

            return new SkeletonBone
            {
                name = boneToRootNames[0],
                position = position,
                rotation = rotation,
                scale = scale,
            };
        }
    }
}