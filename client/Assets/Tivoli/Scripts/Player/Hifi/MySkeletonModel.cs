using UnityEngine;

namespace Tivoli.Scripts.Player.Hifi
{
    public class MySkeletonModel
    {
        private readonly MyAvatar _myAvatar;

        private readonly AnimUtil.CriticallyDampedSpringPoseHelper _smoothHipsHelper = new();

        private readonly Vector3 _tposeHipsPosition;

        public Transform TestHead;
        public Transform TestHips;
        public Transform TestSpine2;

        public MySkeletonModel(MyAvatar myAvatar, Vector3 tposeHipsPosition)
        {
            _myAvatar = myAvatar;
            _tposeHipsPosition = tposeHipsPosition;
        }

        private Vector3 ComputeSpine2WithHeadHipsSpline(AnimPose hipsIkTargetPose, AnimPose headIkTargetPose)
        {
            // the the ik targets to compute the spline with
            var splineFinal = new CubicHermiteSplineFunctorWithArcLength(
                headIkTargetPose.Rot, headIkTargetPose.Trans,
                hipsIkTargetPose.Rot, hipsIkTargetPose.Trans
            );

            // measure the total arc length along the spline
            var totalArcLength = splineFinal.ArcLength(1.0f);
            var tFinal = splineFinal.ArcLengthInverse(_myAvatar.GetSpine2SplineRatio() * totalArcLength);
            var spine2Translation = splineFinal.Evaluate(tFinal);

            return spine2Translation + _myAvatar.GetSpine2SplineOffset();
        }

        private AnimPose ComputeHipsInSensorFrame()
        {
            // TODO: incomplete

            // myAvatar->getCenterOfGravityModelEnabled() &&
            // !isFlying &&
            // !(myAvatar->getIsInWalkingState()) &&
            // !(myAvatar->getIsInSittingState()) &&
            // myAvatar->getHMDLeanRecenterEnabled()
            // then use cg model
            // else deriveBodyFromHMDSensor()
            var hipsMat = _myAvatar.DeriveBodyUsingCgModel();

            var hipsPos = hipsMat.GetPosition();
            var hipsRot = hipsMat.rotation;

            return new AnimPose(hipsRot * AvatarConstants.Y_180, hipsPos);
        }

        public void UpdateRig(float deltaTime, Transform avatarTransform)
        {
            _myAvatar.SetAvatarBonePos(HumanBodyBones.Head, _myAvatar.GetUserEyePosition());
            _myAvatar.SetAvatarBoneRot(HumanBodyBones.Head, _myAvatar.GetUserEyeRotation());

            var sensorHips = ComputeHipsInSensorFrame();
            sensorHips = _smoothHipsHelper.update(sensorHips, deltaTime);
            TestHips.position = sensorHips.Trans + _tposeHipsPosition + avatarTransform.position;
            TestHips.rotation = sensorHips.Rot;

            var sensorHead = new AnimPose(_myAvatar.GetUserEyeRotation(), _myAvatar.GetUserEyePosition());
            
            var spine2TargetTranslation = ComputeSpine2WithHeadHipsSpline(sensorHips, sensorHead);
            TestSpine2.position = spine2TargetTranslation + _tposeHipsPosition + avatarTransform.position;
        }
    }
}