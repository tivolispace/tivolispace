using Tivoli.Scripts.Player;
using UnityEngine;

namespace Tivoli.Scripts.Utils
{
    public class IkDataNetworkCompanion
    {
        // hands or eyes can only be -4 to 4 relative to avatar. this is for compressing
        private const float MAX_NETWORKED_SPACE_SIZE = 4;

        // TODO: should be ushorts wtf
        
        // eye height  y , eye rot    xyzw = 5
        // l hand pos xyz, l hand rot xyzw = 7
        // r hand pos xyz, r hand rot xyzw = 7
        private readonly short[] _compressed = new short[19];

        // ik data, time received
        private (IkData, float) _receivedLast = (null, 0);
        private (IkData, float) _receivedTarget = (new IkData(), 0);

        private readonly IkData _current = new();

        private static short ClampedFloatToShort(float i, float minMaxBothSides)
        {
            return (short) (Mathf.Clamp01(i / minMaxBothSides * 0.5f + 0.5f) * short.MaxValue);
        }

        private static float ClampedShortToFloat(short i, float minMaxBothSides)
        {
            return ((float) i / short.MaxValue * 2 - 1) * minMaxBothSides;
        }

        public IkData Update()
        {
            var last = _receivedLast.Item1;
            if (last == null) return null;
            
            var target = _receivedTarget.Item1;

            var timeLast = _receivedLast.Item2;
            var timeTarget = _receivedTarget.Item2;
            var timeCurrent = Time.time;

            var duration = timeTarget - timeLast;
            var t = duration == 0 ? 0 : (timeCurrent - timeTarget) / duration;

            var preCurrent = _current.Clone();

            _current.LocalEyeHeight = Mathf.Lerp(last.LocalEyeHeight, target.LocalEyeHeight, t);
            _current.LocalEyeHeight = Mathf.Lerp(preCurrent.LocalEyeHeight, _current.LocalEyeHeight, 0.5f);

            _current.LocalLeftHandPosition =
                Vector3.Lerp(last.LocalLeftHandPosition, target.LocalLeftHandPosition, t);
            _current.LocalLeftHandPosition =
                Vector3.Lerp(preCurrent.LocalLeftHandPosition, _current.LocalLeftHandPosition, 0.5f);

            _current.LocalRightHandPosition =
                Vector3.Lerp(last.LocalRightHandPosition, target.LocalRightHandPosition, t);
            _current.LocalRightHandPosition =
                Vector3.Lerp(preCurrent.LocalRightHandPosition, _current.LocalRightHandPosition, 0.5f);

            _current.EyeRotation = Quaternion.Lerp(last.EyeRotation, target.EyeRotation, t);
            _current.EyeRotation = Quaternion.Lerp(preCurrent.EyeRotation, _current.EyeRotation, 0.5f);

            _current.LocalLeftHandRotation =
                Quaternion.Lerp(last.LocalLeftHandRotation, target.LocalLeftHandRotation, t);
            _current.LocalLeftHandRotation =
                Quaternion.Lerp(preCurrent.LocalLeftHandRotation, _current.LocalLeftHandRotation, 0.5f);

            _current.LocalRightHandRotation =
                Quaternion.Lerp(last.LocalRightHandRotation, target.LocalRightHandRotation, t);
            _current.LocalRightHandRotation =
                Quaternion.Lerp(preCurrent.LocalRightHandRotation, _current.LocalRightHandRotation, 0.5f);

            return _current;
        }

        public short[] Compress(IkData ikData)
        {
            _compressed[0] = ClampedFloatToShort(ikData.LocalEyeHeight, MAX_NETWORKED_SPACE_SIZE);

            _compressed[1] = ClampedFloatToShort(ikData.LocalLeftHandPosition.x, MAX_NETWORKED_SPACE_SIZE);
            _compressed[2] = ClampedFloatToShort(ikData.LocalLeftHandPosition.y, MAX_NETWORKED_SPACE_SIZE);
            _compressed[3] = ClampedFloatToShort(ikData.LocalLeftHandPosition.z, MAX_NETWORKED_SPACE_SIZE);

            _compressed[4] = ClampedFloatToShort(ikData.LocalRightHandPosition.x, MAX_NETWORKED_SPACE_SIZE);
            _compressed[5] = ClampedFloatToShort(ikData.LocalRightHandPosition.y, MAX_NETWORKED_SPACE_SIZE);
            _compressed[6] = ClampedFloatToShort(ikData.LocalRightHandPosition.z, MAX_NETWORKED_SPACE_SIZE);

            _compressed[7] = ClampedFloatToShort(ikData.EyeRotation.x, 1);
            _compressed[8] = ClampedFloatToShort(ikData.EyeRotation.y, 1);
            _compressed[9] = ClampedFloatToShort(ikData.EyeRotation.z, 1);
            _compressed[10] = ClampedFloatToShort(ikData.EyeRotation.w, 1);

            _compressed[11] = ClampedFloatToShort(ikData.LocalLeftHandRotation.x, 1);
            _compressed[12] = ClampedFloatToShort(ikData.LocalLeftHandRotation.y, 1);
            _compressed[13] = ClampedFloatToShort(ikData.LocalLeftHandRotation.z, 1);
            _compressed[14] = ClampedFloatToShort(ikData.LocalLeftHandRotation.w, 1);

            _compressed[15] = ClampedFloatToShort(ikData.LocalRightHandRotation.x, 1);
            _compressed[16] = ClampedFloatToShort(ikData.LocalRightHandRotation.y, 1);
            _compressed[17] = ClampedFloatToShort(ikData.LocalRightHandRotation.z, 1);
            _compressed[18] = ClampedFloatToShort(ikData.LocalRightHandRotation.w, 1);

            return _compressed;
        }

        public void ReceiveCompressed(short[] compressed)
        {
            _receivedLast = _receivedTarget;
            
            var target = _receivedTarget.Item1;

            target.LocalEyeHeight = ClampedShortToFloat(compressed[0], MAX_NETWORKED_SPACE_SIZE);

            target.LocalLeftHandPosition = new Vector3(
                ClampedShortToFloat(compressed[1], MAX_NETWORKED_SPACE_SIZE),
                ClampedShortToFloat(compressed[2], MAX_NETWORKED_SPACE_SIZE),
                ClampedShortToFloat(compressed[3], MAX_NETWORKED_SPACE_SIZE)
            );

            target.LocalRightHandPosition = new Vector3(
                ClampedShortToFloat(compressed[4], MAX_NETWORKED_SPACE_SIZE),
                ClampedShortToFloat(compressed[5], MAX_NETWORKED_SPACE_SIZE),
                ClampedShortToFloat(compressed[6], MAX_NETWORKED_SPACE_SIZE)
            );

            target.EyeRotation = new Quaternion(
                ClampedShortToFloat(compressed[7], 1),
                ClampedShortToFloat(compressed[8], 1),
                ClampedShortToFloat(compressed[9], 1),
                ClampedShortToFloat(compressed[10], 1)
            );

            target.LocalLeftHandRotation = new Quaternion(
                ClampedShortToFloat(compressed[11], 1),
                ClampedShortToFloat(compressed[12], 1),
                ClampedShortToFloat(compressed[13], 1),
                ClampedShortToFloat(compressed[14], 1)
            );

            target.LocalRightHandRotation = new Quaternion(
                ClampedShortToFloat(compressed[15], 1),
                ClampedShortToFloat(compressed[16], 1),
                ClampedShortToFloat(compressed[17], 1),
                ClampedShortToFloat(compressed[18], 1)
            );

            _receivedTarget.Item2 = Time.time;
        }
    }
}