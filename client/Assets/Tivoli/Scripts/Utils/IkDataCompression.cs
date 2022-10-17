using Tivoli.Scripts.Player;
using UnityEngine;

namespace Tivoli.Scripts.Utils
{
    public static class IkDataCompression
    {
        // hands or eyes can only be -4 to 4 relative to avatar. this is for compressing
        private const float MAX_NETWORKED_SPACE_SIZE = 4;

        private static short ClampedFloatToShort(float i, float minMaxBothSides)
        {
            return (short) (Mathf.Clamp01(i / minMaxBothSides * 0.5f + 0.5f) * short.MaxValue);
        }

        private static float ClampedShortToFloat(short i, float minMaxBothSides)
        {
            return ((float) i / short.MaxValue * 2 - 1) * minMaxBothSides;
        }

        public static short[] Compress(VrPlayerController.IkData ikData)
        {
            // eye height  y , eye rot    xyzw = 5
            // l hand pos xyz, l hand rot xyzw = 7
            // r hand pos xyz, r hand rot xyzw = 7
            
            var compressed = new short[19];

            compressed[0] = ClampedFloatToShort(ikData.LocalEyeHeight, MAX_NETWORKED_SPACE_SIZE);

            compressed[1] = ClampedFloatToShort(ikData.LocalLeftHandPosition.x, MAX_NETWORKED_SPACE_SIZE);
            compressed[2] = ClampedFloatToShort(ikData.LocalLeftHandPosition.y, MAX_NETWORKED_SPACE_SIZE);
            compressed[3] = ClampedFloatToShort(ikData.LocalLeftHandPosition.z, MAX_NETWORKED_SPACE_SIZE);

            compressed[4] = ClampedFloatToShort(ikData.LocalRightHandPosition.x, MAX_NETWORKED_SPACE_SIZE);
            compressed[5] = ClampedFloatToShort(ikData.LocalRightHandPosition.y, MAX_NETWORKED_SPACE_SIZE);
            compressed[6] = ClampedFloatToShort(ikData.LocalRightHandPosition.z, MAX_NETWORKED_SPACE_SIZE);

            compressed[7] = ClampedFloatToShort(ikData.EyeRotation.x, 1);
            compressed[8] = ClampedFloatToShort(ikData.EyeRotation.y, 1);
            compressed[9] = ClampedFloatToShort(ikData.EyeRotation.z, 1);
            compressed[10] = ClampedFloatToShort(ikData.EyeRotation.w, 1);

            compressed[11] = ClampedFloatToShort(ikData.LocalLeftHandRotation.x, 1);
            compressed[12] = ClampedFloatToShort(ikData.LocalLeftHandRotation.y, 1);
            compressed[13] = ClampedFloatToShort(ikData.LocalLeftHandRotation.z, 1);
            compressed[14] = ClampedFloatToShort(ikData.LocalLeftHandRotation.w, 1);

            compressed[15] = ClampedFloatToShort(ikData.LocalRightHandRotation.x, 1);
            compressed[16] = ClampedFloatToShort(ikData.LocalRightHandRotation.y, 1);
            compressed[17] = ClampedFloatToShort(ikData.LocalRightHandRotation.z, 1);
            compressed[18] = ClampedFloatToShort(ikData.LocalRightHandRotation.w, 1);

            return compressed;
        }

        public static VrPlayerController.IkData Decompress(short[] compressed)
        {
            return new VrPlayerController.IkData
            {
                LocalEyeHeight = ClampedShortToFloat(compressed[0], MAX_NETWORKED_SPACE_SIZE),

                LocalLeftHandPosition = new Vector3(
                    ClampedShortToFloat(compressed[1], MAX_NETWORKED_SPACE_SIZE),
                    ClampedShortToFloat(compressed[2], MAX_NETWORKED_SPACE_SIZE),
                    ClampedShortToFloat(compressed[3], MAX_NETWORKED_SPACE_SIZE)
                ),

                LocalRightHandPosition = new Vector3(
                    ClampedShortToFloat(compressed[4], MAX_NETWORKED_SPACE_SIZE),
                    ClampedShortToFloat(compressed[5], MAX_NETWORKED_SPACE_SIZE),
                    ClampedShortToFloat(compressed[6], MAX_NETWORKED_SPACE_SIZE)
                ),

                EyeRotation = new Quaternion(
                    ClampedShortToFloat(compressed[7], 1),
                    ClampedShortToFloat(compressed[8], 1),
                    ClampedShortToFloat(compressed[9], 1),
                    ClampedShortToFloat(compressed[10], 1)
                ),

                LocalLeftHandRotation = new Quaternion(
                    ClampedShortToFloat(compressed[11], 1),
                    ClampedShortToFloat(compressed[12], 1),
                    ClampedShortToFloat(compressed[13], 1),
                    ClampedShortToFloat(compressed[14], 1)
                ),

                LocalRightHandRotation = new Quaternion(
                    ClampedShortToFloat(compressed[15], 1),
                    ClampedShortToFloat(compressed[16], 1),
                    ClampedShortToFloat(compressed[17], 1),
                    ClampedShortToFloat(compressed[18], 1)
                ),
            };
        }
    }
}