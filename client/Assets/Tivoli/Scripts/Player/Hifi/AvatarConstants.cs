using UnityEngine;

namespace Tivoli.Scripts.Player.Hifi
{
    public static class AvatarConstants
    {
        public static readonly Vector3 UNIT_X = new(1.0f, 0.0f, 0.0f);
        public static readonly Vector3 UNIT_Y = new(0.0f, 1.0f, 0.0f);
        public static readonly Vector3 UNIT_Z = new(0.0f, 0.0f, 1.0f);

        public static readonly Quaternion X_180 = new(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Quaternion Y_180 = new(0.0f, 0.0f, 1.0f, 0.0f);
        public static readonly Quaternion Z_180 = new(0.0f, 0.0f, 0.0f, 1.0f);

        // 50th Percentile Man
        public const float DEFAULT_AVATAR_HEIGHT = 1.755f; // meters
        public const float DEFAULT_AVATAR_EYE_TO_TOP_OF_HEAD = 0.11f; // meters
        public const float DEFAULT_AVATAR_NECK_TO_TOP_OF_HEAD = 0.185f; // meters
        public const float DEFAULT_AVATAR_NECK_HEIGHT = DEFAULT_AVATAR_HEIGHT - DEFAULT_AVATAR_NECK_TO_TOP_OF_HEAD;
        public const float DEFAULT_AVATAR_EYE_HEIGHT = DEFAULT_AVATAR_HEIGHT - DEFAULT_AVATAR_EYE_TO_TOP_OF_HEAD;
        public const float DEFAULT_SPINE2_SPLINE_PROPORTION = 0.71f;
        public const float DEFAULT_AVATAR_SUPPORT_BASE_LEFT = -0.25f;
        public const float DEFAULT_AVATAR_SUPPORT_BASE_RIGHT = 0.25f;
        public const float DEFAULT_AVATAR_SUPPORT_BASE_FRONT = -0.20f;
        public const float DEFAULT_AVATAR_SUPPORT_BASE_BACK = 0.12f;
        public const float DEFAULT_AVATAR_LATERAL_STEPPING_THRESHOLD = 0.10f;
        public const float DEFAULT_AVATAR_ANTERIOR_STEPPING_THRESHOLD = 0.04f;
        public const float DEFAULT_AVATAR_POSTERIOR_STEPPING_THRESHOLD = 0.05f;
        public const float DEFAULT_AVATAR_HEAD_ANGULAR_VELOCITY_STEPPING_THRESHOLD = 0.3f;
        public const float DEFAULT_AVATAR_MODE_HEIGHT_STEPPING_THRESHOLD = -0.02f;
        public const float DEFAULT_HANDS_VELOCITY_DIRECTION_STEPPING_THRESHOLD = 0.4f;
        public const float DEFAULT_HANDS_ANGULAR_VELOCITY_STEPPING_THRESHOLD = 3.3f;
        public const float DEFAULT_HEAD_VELOCITY_STEPPING_THRESHOLD = 0.18f;
        public const float DEFAULT_HEAD_PITCH_STEPPING_TOLERANCE = 7.0f;
        public const float DEFAULT_HEAD_ROLL_STEPPING_TOLERANCE = 7.0f;
        public const float DEFAULT_AVATAR_SPINE_STRETCH_LIMIT = 0.04f;
        public const float DEFAULT_AVATAR_FORWARD_DAMPENING_FACTOR = 0.5f;
        public const float DEFAULT_AVATAR_LATERAL_DAMPENING_FACTOR = 2.0f;
        public const float DEFAULT_AVATAR_HIPS_MASS = 40.0f;
        public const float DEFAULT_AVATAR_HEAD_MASS = 20.0f;
        public const float DEFAULT_AVATAR_LEFTHAND_MASS = 2.0f;
        public const float DEFAULT_AVATAR_RIGHTHAND_MASS = 2.0f;
        public const float DEFAULT_AVATAR_IPD = 0.064f;

        // Used when avatar is missing joints... (avatar space)
        public static readonly Quaternion DEFAULT_AVATAR_MIDDLE_EYE_ROT = Y_180;
        public static readonly Vector3 DEFAULT_AVATAR_HEAD_TO_MIDDLE_EYE_OFFSET = new(0.0f, 0.064f, 0.084f);
        public static readonly Vector3 DEFAULT_AVATAR_HEAD_POS = new(0.0f, 0.53f, 0.0f);
        public static readonly Quaternion DEFAULT_AVATAR_HEAD_ROT = Y_180;
        public static readonly Vector3 DEFAULT_AVATAR_RIGHTARM_POS = new(-0.134824f, 0.396348f, -0.0515777f);

        public static readonly Quaternion DEFAULT_AVATAR_RIGHTARM_ROT =
            new(-0.536241f, 0.536241f, -0.460918f, -0.460918f);

        public static readonly Vector3 DEFAULT_AVATAR_LEFTARM_POS = new(0.134795f, 0.396349f, -0.0515881f);
        public static readonly Quaternion DEFAULT_AVATAR_LEFTARM_ROT = new(0.536257f, 0.536258f, -0.460899f, 0.4609f);
        public static readonly Vector3 DEFAULT_AVATAR_RIGHTHAND_POS = new(-0.72768f, 0.396349f, -0.0515779f);

        public static readonly Quaternion DEFAULT_AVATAR_RIGHTHAND_ROT =
            new(0.479184f, -0.520013f, 0.522537f, 0.476365f);

        public static readonly Vector3 DEFAULT_AVATAR_LEFTHAND_POS = new(0.727588f, 0.39635f, -0.0515878f);

        public static readonly Quaternion DEFAULT_AVATAR_LEFTHAND_ROT =
            new(-0.479181f, -0.52001f, 0.52254f, -0.476369f);

        public static readonly Vector3 DEFAULT_AVATAR_NECK_POS = new(0.0f, 0.445f, 0.025f);
        public static readonly Vector3 DEFAULT_AVATAR_SPINE2_POS = new(0.0f, 0.32f, 0.02f);
        public static readonly Quaternion DEFAULT_AVATAR_SPINE2_ROT = Y_180;
        public static readonly Vector3 DEFAULT_AVATAR_HIPS_POS = new(0.0f, 0.0f, 0.0f);
        public static readonly Quaternion DEFAULT_AVATAR_HIPS_ROT = Y_180;
        public static readonly Vector3 DEFAULT_AVATAR_LEFTFOOT_POS = new(-0.08f, -0.96f, 0.029f);

        public static readonly Quaternion DEFAULT_AVATAR_LEFTFOOT_ROT = new(-0.40167322754859924f, 0.9154590368270874f,
            -0.005437685176730156f, -0.023744143545627594f);

        public static readonly Vector3 DEFAULT_AVATAR_RIGHTFOOT_POS = new(0.08f, -0.96f, 0.029f);

        public static readonly Quaternion DEFAULT_AVATAR_RIGHTFOOT_ROT = new(-0.4016716778278351f, 0.9154615998268127f,
            0.0053307069465518f, 0.023696165531873703f);
    }
}