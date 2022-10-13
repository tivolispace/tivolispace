namespace Tivoli.Scripts.Player.Hifi
{
    public class MySkeletonModel
    {
        private readonly MyAvatar _myAvatar;

        private readonly AnimUtil.CriticallyDampedSpringPoseHelper _smoothHipsHelper = new();

        public MySkeletonModel(MyAvatar myAvatar)
        {
            _myAvatar = myAvatar;
        }

        private AnimPose ComputeHipsInSensorFrame()
        {
            // TODO: incomplete

            var hipsMat = _myAvatar.DeriveBodyUsingCgModel();
            // deriveBodyFromHMDSensor();

            var hipsPos = hipsMat.GetPosition();
            var hipsRot = hipsMat.rotation;

            return new AnimPose(hipsRot * AvatarConstants.Y_180, hipsPos);
        }

        public AnimPose UpdateRig(float deltaTime)
        {
            var sensorHips = ComputeHipsInSensorFrame();
            sensorHips = _smoothHipsHelper.update(sensorHips, deltaTime);

            return sensorHips;
        }
    }
}