using System;
using UnityEngine;

namespace Tivoli.Scripts.UI
{
    public static class EasingFunctions
    {
        // https://github.com/ppy/osu-framework/blob/2d4142c5433492868f1ba7a2f670afeb84a9e906/osu.Framework/Graphics/Easing.cs
        // https://github.com/ppy/osu-framework/blob/2d4142c5433492868f1ba7a2f670afeb84a9e906/osu.Framework/Graphics/Transforms/DefaultEasingFunction.cs

        public enum Easing
        {
            None,
            Out,
            In,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InSine,
            OutSine,
            InOutSine,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            OutElasticHalf,
            OutElasticQuarter,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
            OutPow10,
        }

        private const float ElasticConst = 2f * Mathf.PI / .3f;
        private const float ElasticConst2 = .3f / 4f;

        private const float BackConst = 1.70158f;
        private const float BackConst2 = BackConst * 1.525f;

        private const float BounceConst = 1f / 2.75f;

        // constants used to fix expo and elastic curves to start/end at 0/1
        private static readonly float ExpoOffset = Mathf.Pow(2f, -10f);
        private static readonly float ElasticOffsetFull = Mathf.Pow(2f, -11f);

        private static readonly float ElasticOffsetHalf =
            Mathf.Pow(2f, -10f) * Mathf.Sin((.5f - ElasticConst2) * ElasticConst);

        private static readonly float ElasticOffsetQuarter =
            Mathf.Pow(2f, -10f) * Mathf.Sin((.25f - ElasticConst2) * ElasticConst);

        private static readonly float InOutElasticOffset =
            Mathf.Pow(2f, -10f) * Mathf.Sin((1f - ElasticConst2 * 1.5f) * ElasticConst / 1.5f);

        public static float Ease(float t, Easing easing)
        {
            switch (easing)
            {
                default:
                case Easing.None:
                    return t;

                case Easing.In:
                case Easing.InQuad:
                    return t * t;

                case Easing.Out:
                case Easing.OutQuad:
                    return t * (2f - t);

                case Easing.InOutQuad:
                    if (t < .5f) return t * t * 2f;

                    return --t * t * -2f + 1f;

                case Easing.InCubic:
                    return t * t * t;

                case Easing.OutCubic:
                    return --t * t * t + 1f;

                case Easing.InOutCubic:
                    if (t < .5f) return t * t * t * 4f;
                    return --t * t * t * 4f + 1f;

                case Easing.InQuart:
                    return t * t * t * t;

                case Easing.OutQuart:
                    return 1f - --t * t * t * t;

                case Easing.InOutQuart:
                    if (t < .5f) return t * t * t * t * 8f;
                    return --t * t * t * t * -8f + 1f;

                case Easing.InQuint:
                    return t * t * t * t * t;

                case Easing.OutQuint:
                    return --t * t * t * t * t + 1f;

                case Easing.InOutQuint:
                    if (t < .5f) return t * t * t * t * t * 16f;
                    return --t * t * t * t * t * 16f + 1f;

                case Easing.InSine:
                    return 1f - Mathf.Cos(t * Mathf.PI * .5f);

                case Easing.OutSine:
                    return Mathf.Sin(t * Mathf.PI * .5f);

                case Easing.InOutSine:
                    return .5f - .5f * Mathf.Cos(Mathf.PI * t);

                case Easing.InExpo:
                    return Mathf.Pow(2f, 10f * (t - 1f)) + ExpoOffset * (t - 1f);

                case Easing.OutExpo:
                    return -Mathf.Pow(2f, -10f * t) + 1f + ExpoOffset * t;

                case Easing.InOutExpo:
                    if (t < .5f) return .5f * (Mathf.Pow(2f, 20f * t - 10f) + ExpoOffset * (2f * t - 1f));
                    return 1f - .5f * (Mathf.Pow(2f, -20f * t + 10f) + ExpoOffset * (-2f * t + 1f));

                case Easing.InCirc:
                    return 1f - Mathf.Sqrt(1f - t * t);

                case Easing.OutCirc:
                    return Mathf.Sqrt(1f - --t * t);

                case Easing.InOutCirc:
                    if ((t *= 2f) < 1f) return .5f - .5f * Mathf.Sqrt(1f - t * t);
                    return .5f * Mathf.Sqrt(1f - (t -= 2f) * t) + .5f;

                case Easing.InElastic:
                    return -Mathf.Pow(2f, -10f + 10f * t) * Mathf.Sin((1f - ElasticConst2 - t) * ElasticConst) +
                           ElasticOffsetFull * (1f - t);

                case Easing.OutElastic:
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - ElasticConst2) * ElasticConst) + 1f -
                           ElasticOffsetFull * t;

                case Easing.OutElasticHalf:
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((.5f * t - ElasticConst2) * ElasticConst) + 1f -
                           ElasticOffsetHalf * t;

                case Easing.OutElasticQuarter:
                    return Mathf.Pow(2f, -10 * t) * Mathf.Sin((.25f * t - ElasticConst2) * ElasticConst) + 1f -
                           ElasticOffsetQuarter * t;

                case Easing.InOutElastic:
                    if ((t *= 2) < 1)
                    {
                        return -.5f * (Mathf.Pow(2f, -10f + 10f * t) *
                                       Mathf.Sin((1f - ElasticConst2 * 1.5f - t) * ElasticConst / 1.5f)
                                       - InOutElasticOffset * (1f - t));
                    }

                    return .5f * (Mathf.Pow(2f, -10f * --t) *
                                  Mathf.Sin((t - ElasticConst2 * 1.5f) * ElasticConst / 1.5f)
                                  - InOutElasticOffset * t) + 1f;

                case Easing.InBack:
                    return t * t * ((BackConst + 1f) * t - BackConst);

                case Easing.OutBack:
                    return --t * t * ((BackConst + 1f) * t + BackConst) + 1f;

                case Easing.InOutBack:
                    if ((t *= 2f) < 1f) return .5f * t * t * ((BackConst2 + 1f) * t - BackConst2);
                    return .5f * ((t -= 2f) * t * ((BackConst2 + 1f) * t + BackConst2) + 2f);

                case Easing.InBounce:
                    t = 1f - t;
                    return t switch
                    {
                        < BounceConst => 1f - 7.5625f * t * t,
                        < 2f * BounceConst => 1f - (7.5625f * (t -= 1.5f * BounceConst) * t + .75f),
                        < 2.5f * BounceConst => 1f - (7.5625f * (t -= 2.25f * BounceConst) * t + .9375f),
                        _ => 1f - (7.5625f * (t -= 2.625f * BounceConst) * t + .984375f)
                    };

                case Easing.OutBounce:
                    return t switch
                    {
                        < BounceConst => 7.5625f * t * t,
                        < 2f * BounceConst => 7.5625f * (t -= 1.5f * BounceConst) * t + .75f,
                        < 2.5f * BounceConst => 7.5625f * (t -= 2.25f * BounceConst) * t + .9375f,
                        _ => 7.5625f * (t -= 2.625f * BounceConst) * t + .984375f
                    };

                case Easing.InOutBounce:
                    if (t < .5f)
                        return .5f - .5f * Ease(1f - t * 2f, Easing.OutBounce);
                    return Ease((t - .5f) * 2f, Easing.OutBounce) * .5f + .5f;

                case Easing.OutPow10:
                    return --t * Mathf.Pow(t, 10f) + 1f;
            }
        }
    }
}