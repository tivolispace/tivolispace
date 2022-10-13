using System;
using UnityEngine;
using static Tivoli.Scripts.Player.Hifi.AnimUtil;
using static Tivoli.Scripts.Player.Hifi.AvatarConstants;

namespace Tivoli.Scripts.Player.Hifi
{
    public class MyAvatar
    {
        public Func<HumanBodyBones, Vector3> GetAvatarBonePos = _ => Vector3.zero;
        public Action<HumanBodyBones, Vector3> SetAvatarBonePos = (_, _) => { };
        public Func<HumanBodyBones, Vector3> GetAvatarDefaultBonePos = _ => Vector3.zero;
        
        public Func<HumanBodyBones, Quaternion> GetAvatarBoneRot = _ => Quaternion.identity;
        public Action<HumanBodyBones, Quaternion> SetAvatarBoneRot = (_, _) => { };
        public Func<HumanBodyBones, Quaternion> GetAvatarDefaultBoneRot = _ => Quaternion.identity;

        public Func<Vector3> GetUserEyePosition = () => Vector3.zero;
        public Func<Quaternion> GetUserEyeRotation = () => Quaternion.identity;

        private float GetUserEyeHeight() => GetUserEyePosition().y;

        private float _spine2SplineRatio = DEFAULT_SPINE2_SPLINE_PROPORTION;
        public float GetSpine2SplineRatio() => _spine2SplineRatio;

        private Vector3 _spine2SplineOffset;
        public Vector3 GetSpine2SplineOffset() => _spine2SplineOffset;

        private void BuildSpine2SplineRatioCache()
        {
            var hipsRigDefaultPose = new AnimPose(GetAvatarDefaultBoneRot(HumanBodyBones.Hips),
                GetAvatarDefaultBonePos(HumanBodyBones.Hips));
            var headRigDefaultPose = new AnimPose(GetAvatarDefaultBoneRot(HumanBodyBones.Head),
                GetAvatarDefaultBonePos(HumanBodyBones.Head));
            var basePosition = hipsRigDefaultPose.Trans;
            var tipPosition = headRigDefaultPose.Trans;
            var spine2Position = GetAvatarDefaultBonePos(HumanBodyBones.UpperChest); // spine2 is upper chest

            var baseToTip = tipPosition - basePosition;
            var baseToTipLength = Vector3.Magnitude(baseToTip);
            var baseToTipNormal = baseToTip / baseToTipLength;
            var baseToSpine2 = spine2Position - basePosition;

            _spine2SplineRatio = Vector3.Dot(baseToSpine2, baseToTipNormal) / baseToTipLength;

            var defaultSpline = new CubicHermiteSplineFunctorWithArcLength(
                headRigDefaultPose.Rot, headRigDefaultPose.Trans,
                hipsRigDefaultPose.Rot, hipsRigDefaultPose.Trans
            );

            // measure the total arc length along the spline
            var totalDefaultArcLength = defaultSpline.ArcLength(1.0f);
            var t = defaultSpline.ArcLengthInverse(_spine2SplineRatio * totalDefaultArcLength);
            var defaultSplineSpine2Translation = defaultSpline.Evaluate(t);

            _spine2SplineOffset = spine2Position - defaultSplineSpine2Translation;
        }

        public void RigReady()
        {
            // buildUnscaledEyeHeightCache();
            BuildSpine2SplineRatioCache();
            // computeMultiSphereShapes();
            // buildSpine2SplineRatioCache();
            // setSkeletonData(getSkeletonDefaultData());
            // sendSkeletonData();
        }


        // ease in function for dampening cg movement
        private static float Slope(float num)
        {
            const float curveConstant = 1.0f;
            var ret = 1.0f;
            if (num > 0.0f)
            {
                ret = 1.0f - (1.0f / (1.0f + curveConstant * num));
            }

            return ret;
        }

        // This function gives a soft clamp at the edge of the base of support
        // dampenCgMovement returns the damped cg value in Avatar space.
        // cgUnderHeadHandsAvatarSpace is also in Avatar space
        // baseOfSupportScale is based on the height of the user
        private static Vector3 DampenCgMovement(Vector3 cgUnderHeadHandsAvatarSpace, float baseOfSupportScale)
        {
            var distanceFromCenterZ = cgUnderHeadHandsAvatarSpace.z;
            var distanceFromCenterX = cgUnderHeadHandsAvatarSpace.x;

            // In the forward direction we need a different scale because forward is in
            // the direction of the hip extensor joint, which means bending usually happens
            // well before reaching the edge of the base of support.
            var clampFront =
                DEFAULT_AVATAR_SUPPORT_BASE_FRONT * DEFAULT_AVATAR_FORWARD_DAMPENING_FACTOR * baseOfSupportScale;
            var clampBack =
                DEFAULT_AVATAR_SUPPORT_BASE_BACK * DEFAULT_AVATAR_LATERAL_DAMPENING_FACTOR * baseOfSupportScale;
            var clampLeft =
                DEFAULT_AVATAR_SUPPORT_BASE_LEFT * DEFAULT_AVATAR_LATERAL_DAMPENING_FACTOR * baseOfSupportScale;
            var clampRight =
                DEFAULT_AVATAR_SUPPORT_BASE_RIGHT * DEFAULT_AVATAR_LATERAL_DAMPENING_FACTOR * baseOfSupportScale;
            var dampedCg = new Vector3(0.0f, 0.0f, 0.0f);

            // find the damped z coord of the cg
            if (cgUnderHeadHandsAvatarSpace.z < 0.0f)
            {
                // forward displacement
                dampedCg.z = Slope(Mathf.Abs(distanceFromCenterZ / clampFront)) * clampFront;
            }
            else
            {
                // backwards displacement
                dampedCg.z = Slope(Mathf.Abs(distanceFromCenterZ / clampBack)) * clampBack;
            }

            // find the damped x coord of the cg
            if (cgUnderHeadHandsAvatarSpace.x > 0.0f)
            {
                // right of center
                dampedCg.x = Slope(Mathf.Abs(distanceFromCenterX / clampRight)) * clampRight;
            }
            else
            {
                // left of center
                dampedCg.x = Slope(Mathf.Abs(distanceFromCenterX / clampLeft)) * clampLeft;
            }

            return dampedCg;
        }

        private class JointMass
        {
            public string Name;
            public float Weight;
            public Vector3 Position;
        }

        // computeCounterBalance returns the center of gravity in Avatar space
        private Vector3 ComputeCounterBalance()
        {
            var cgHeadMass = new JointMass
            {
                Name = "Head", Weight = DEFAULT_AVATAR_HEAD_MASS,
                Position = GetAvatarBonePos(HumanBodyBones.Head)
            };
            var cgLeftHandMass = new JointMass
            {
                Name = "LeftHand", Weight = DEFAULT_AVATAR_LEFTHAND_MASS,
                Position = GetAvatarBonePos(HumanBodyBones.LeftHand)
            };
            var cgRightHandMass = new JointMass
            {
                Name = "RightHand", Weight = DEFAULT_AVATAR_RIGHTHAND_MASS,
                Position = GetAvatarBonePos(HumanBodyBones.RightHand)
            };

            var tposeHead = GetAvatarDefaultBonePos(HumanBodyBones.Head); // or DEFAULT_AVATAR_HEAD_POS;
            var tposeHips = GetAvatarDefaultBonePos(HumanBodyBones.Hips); // or DEFAULT_AVATAR_HIPS_POS;
            // var tposeRightFoot = DEFAULT_AVATAR_RIGHTFOOT_POS;

            // find the current center of gravity position based on head and hand moments
            var sumOfMoments = (cgHeadMass.Weight * cgHeadMass.Position) +
                               (cgLeftHandMass.Weight * cgLeftHandMass.Position) +
                               (cgRightHandMass.Weight * cgRightHandMass.Position);
            var totalMass = cgHeadMass.Weight + cgLeftHandMass.Weight + cgRightHandMass.Weight;

            var currentCg = (1.0f / totalMass) * sumOfMoments;
            currentCg.y = 0;

            // dampening the center of gravity, in effect, limits the value to the perimeter of the base of support
            var baseScale = 1.0f;
            if (GetUserEyeHeight() > 0.0f)
            {
                baseScale = GetUserEyeHeight() / DEFAULT_AVATAR_EYE_HEIGHT;
            }

            var desiredCg = DampenCgMovement(currentCg, baseScale);

            // compute hips position to maintain desiredCg
            var counterBalancedForHead = (totalMass + DEFAULT_AVATAR_HIPS_MASS) * desiredCg;
            counterBalancedForHead -= sumOfMoments;
            var counterBalancedCg = (1.0f / DEFAULT_AVATAR_HIPS_MASS) * counterBalancedForHead;

            // find the height of the hips
            var xzDiff = new Vector3(cgHeadMass.Position.x - counterBalancedCg.x, 0.0f,
                cgHeadMass.Position.z - counterBalancedCg.z);
            var headMinusHipXz = Vector3.Magnitude(xzDiff);
            var headHipDefault = Vector3.Magnitude(tposeHead - tposeHips);
            var hipHeight = 0.0f;
            if (headHipDefault > headMinusHipXz)
            {
                hipHeight = Mathf.Sqrt((headHipDefault * headHipDefault) - (headMinusHipXz * headMinusHipXz));
            }

            counterBalancedCg.y = (cgHeadMass.Position.y - hipHeight);

            // this is to be sure that the feet don't lift off the floor.
            // add 5 centimeters to allow for going up on the toes.
            if (counterBalancedCg.y > (tposeHips.y + 0.05f))
            {
                // if the height is higher than default hips, clamp to default hips
                counterBalancedCg.y = tposeHips.y + 0.05f;
            }

            return counterBalancedCg;
        }

        // this function matches the hips rotation to the new cghips-head axis
        // headOrientation, headPosition and hipsPosition are in avatar space
        // returns the matrix of the hips in Avatar space
        // static glm::mat4 computeNewHipsMatrix(glm::quat headOrientation, glm::vec3 headPosition, glm::vec3 hipsPosition) {
        private static Matrix4x4 ComputeNewHipsMatrix(Quaternion headOrientation, Vector3 headPosition,
            Vector3 hipsPosition)
        {
            var bodyOrientation = ComputeBodyFacingFromHead(headOrientation, UNIT_Y);

            const float MIX_RATIO = 0.3f;
            var hipsRot = SafeLerp(Quaternion.identity, bodyOrientation, MIX_RATIO);
            var hipsFacing = hipsRot * UNIT_Z;

            var spineVec = headPosition - hipsPosition;

            return Matrix4x4.TRS(
                hipsPosition,
                Quaternion.LookRotation(hipsFacing, Vector3.Normalize(spineVec)),
                Vector3.one
            );
        }

        // this function finds the hips position using a center of gravity model that
        // balances the head and hands with the hips over the base of support
        // returns the rotation (-z forward) and position of the Avatar in Sensor space
        public Matrix4x4 DeriveBodyUsingCgModel()
        {
            // get the new center of gravity
            var cgHipsPosition = ComputeCounterBalance();

            // find the new hips rotation using the new head-hips axis as the up axis
            // TODO: it should be head pos and rot maybe
            var avatarHipsMat = ComputeNewHipsMatrix(
                GetUserEyeRotation(), GetUserEyePosition(), cgHipsPosition
            );

            // // convert hips from avatar to sensor space
            // // The Y_180 is to convert from z forward to -z forward.
            // return worldToSensorMat * avatarToWorldMat * avatarHipsMat;
            return avatarHipsMat;
        }
    }
}