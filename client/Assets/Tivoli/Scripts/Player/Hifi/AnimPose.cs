using UnityEngine;

namespace Tivoli.Scripts.Player.Hifi
{
    public class AnimPose
    {
        public Vector3 Scale = Vector3.one;
        public Quaternion Rot = Quaternion.identity;
        public Vector3 Trans = Vector3.zero;

        public AnimPose(Matrix4x4 mat)
        {
            Scale = mat.lossyScale;
            Rot = mat.rotation;
            Trans = mat.GetPosition();
        }

        public AnimPose(Quaternion rot)
        {
            Rot = rot;
        }

        public AnimPose(Quaternion rot, Vector3 trans)
        {
            Rot = rot;
            Trans = trans;
        }

        public AnimPose(Vector3 scale, Quaternion rot, Vector3 trans)
        {
            Scale = scale;
            Rot = rot;
            Trans = trans;
        }

        public AnimPose(AnimPose pose)
        {
            Scale = pose.Scale;
            Rot = pose.Rot;
            Trans = pose.Trans;
        }
    }
}