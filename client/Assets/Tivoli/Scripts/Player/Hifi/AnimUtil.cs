using UnityEngine;
using static Tivoli.Scripts.Player.Hifi.AvatarConstants;

namespace Tivoli.Scripts.Player.Hifi
{
    public static class AnimUtil
    {
        public static Quaternion SafeLerp(Quaternion a, Quaternion b, float alpha)
        {
            // // adjust signs if necessary
            // var bTemp = b;
            // var dot = Quaternion.Dot(a, bTemp);
            // if (dot < 0.0f)
            // {
            //     bTemp = -bTemp;
            // }
            // return Quaternion.Normalize(Quaternion.Lerp(a, bTemp, alpha));

            // theres also LerpUnclamped which might be unsafe?
            return Quaternion.Lerp(a, b, alpha);
        }

        // This will attempt to determine the proper body facing of a characters body
        // assumes headRot is z-forward and y-up.
        // and returns a bodyRot that is also z-forward and y-up
        public static Quaternion ComputeBodyFacingFromHead(Quaternion headRot, Vector3 up)
        {
            var bodyUp = Vector3.Normalize(up);

            // initially take the body facing from the head.
            var headUp = headRot * UNIT_Y;
            var headForward = headRot * UNIT_Z;
            var headLeft = headRot * UNIT_X;
            var NOD_THRESHOLD = Mathf.Cos(Mathf.Deg2Rad * 45.0f);
            var TILT_THRESHOLD = Mathf.Cos(Mathf.Deg2Rad * 30.0f);

            var bodyForward = headForward;

            var nodDot = Vector3.Dot(headForward, bodyUp);
            var tiltDot = Vector3.Dot(headLeft, bodyUp);

            if (Mathf.Abs(tiltDot) < TILT_THRESHOLD) // if we are not tilting too much
            {
                if (nodDot < -NOD_THRESHOLD) // head is looking downward
                {
                    // the body should face in the same direction as the top the head.
                    bodyForward = headUp;
                }
                else if (nodDot > NOD_THRESHOLD) // head is looking upward
                {
                    // the body should face away from the top of the head.
                    bodyForward = -headUp;
                }
            }

            // cancel out upward component
            bodyForward = Vector3.Normalize(bodyForward - nodDot * bodyUp);

            return Quaternion.LookRotation(bodyForward, bodyUp);
        }

        public class CriticallyDampedSpringPoseHelper
        {
            private AnimPose _prevPose;
            private float _horizontalTranslationTimescale = 0.15f;
            private float _verticalTranslationTimescale = 0.15f;
            private float _rotationTimescale = 0.15f;
            private bool _prevPoseValid = false;

            public void SetHorizontalTranslationTimescale(float timescale)
            {
                _horizontalTranslationTimescale = timescale;
            }

            public void SetVerticalTranslationTimescale(float timescale)
            {
                _verticalTranslationTimescale = timescale;
            }

            public void SetRotationTimescale(float timescale)
            {
                _rotationTimescale = timescale;
            }

            public AnimPose update(AnimPose pose, float deltaTime)
            {
                if (!_prevPoseValid)
                {
                    _prevPose = pose;
                    _prevPoseValid = true;
                }

                var horizontalTranslationAlpha = Mathf.Min(deltaTime / _horizontalTranslationTimescale, 1.0f);
                var verticalTranslationAlpha = Mathf.Min(deltaTime / _verticalTranslationTimescale, 1.0f);
                var rotationAlpha = Mathf.Min(deltaTime / _rotationTimescale, 1.0f);

                var poseY = pose.Trans.y;
                var newPose = new AnimPose(_prevPose);
                newPose.Trans = Vector3.Lerp(_prevPose.Trans, pose.Trans, horizontalTranslationAlpha);
                newPose.Trans.y = Mathf.Lerp(_prevPose.Trans.y, poseY, verticalTranslationAlpha);
                newPose.Rot = Quaternion.Lerp(_prevPose.Rot, pose.Rot, rotationAlpha);

                _prevPose = newPose;
                _prevPoseValid = true;

                return newPose;
            }

            public void Teleport(AnimPose pose)
            {
                _prevPoseValid = true;
                _prevPose = pose;
            }
        }
    }
}