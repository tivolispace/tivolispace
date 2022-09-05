using System;
using UnityEngine;

namespace Tivoli.Scripts.UI
{
    public class EasingFunctions
    {
        public enum Easing
        {
            Linear,
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
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
            InElastic,
            OutElastic,
            InOutElastic,
            MaterialStandard,
            MaterialDecelerate,
            MaterialAccelerate,
        }

        // https://github.com/ai/easings.net/blob/master/src/easings/easingsFunctions.ts
        
        public static float Linear(float x)
        {
            return x;
        }

        public static float InQuad(float x)
        {
            return x * x;
        }

        public static float OutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        public static float InOutQuad(float x)
        {
            return x switch
            {
                < 0.5f => 2f * x * x,
                _ => 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f
            };
        }

        public static float InCubic(float x)
        {
            return x * x * x;
        }

        public static float OutCubic(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3);
        }

        public static float InOutCubic(float x)
        {
            return x switch
            {
                < 0.5f => 4f * x * x * x,
                _ => 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f
            };
        }

        public static float InQuart(float x)
        {
            return x * x * x * x;
        }

        public static float OutQuart(float x)
        {
            return 1f - Mathf.Pow(1f - x, 4f);
        }

        public static float InOutQuart(float x)
        {
            return x switch
            {
                < 0.5f => 8f * x * x * x * x,
                _ => 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f
            };
        }

        public static float InQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float OutQuint(float x)
        {
            return 1f - Mathf.Pow(1f - x, 5f);
        }

        public static float InOutQuint(float x)
        {
            return x switch
            {
                < 0.5f => 16f * x * x * x * x * x,
                _ => 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f
            };
        }

        public static float InSine(float x)
        {
            return 1f - Mathf.Cos((x * Mathf.PI) / 2f);
        }
        
        public static float OutSine(float x)
        {
            return Mathf.Sin((x * Mathf.PI) / 2f);
        }
        
        public static float InOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }

        public static float InExpo(float x)
        {
            return x switch
            {
                0f => 0f,
                _ => Mathf.Pow(2f, 10f * x - 10f),
            };
        }
        
        public static float OutExpo(float x)
        {
            return x switch
            {
                1f => 1f,
                _ => 1f - Mathf.Pow(2f, -10f * x),
            };
        }
        
        public static float InOutExpo(float x)
        {
            return x switch
            {
                0f => 0f,
                1f => 1f,
                < 0.5f => Mathf.Pow(2f, 20f * x - 10f) / 2f,
                _ => (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f
            };
        }

        public static float InCirc(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
        }
        
        public static float OutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }
        
        public static float InOutCirc(float x)
        {
            return x switch
            {
                < 0.5f => (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f,
                _ => (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f
            };
        }

        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = C1 + 1f;
        private const float C4 = (2f * Mathf.PI) / 3f;
        private const float C5 = (2f * Mathf.PI) / 4.5f;

        public static float InBack(float x)
        {
            return C3 * x * x * x - C1 * x * x;
        }
        
        public static float OutBack(float x)
        {
            return 1f + C3 * Mathf.Pow(x - 1f, 3f) + C1 * Mathf.Pow(x - 1f, 2f);
        }

        public static float InOutBack(float x)
        {
            return x switch
            {
                < 0.5f => (Mathf.Pow(2f * x, 2f) * ((C2 + 1f) * 2f * x - C2)) / 2f,
                _ => (Mathf.Pow(2f * x - 2f, 2f) * ((C2 + 1f) * (x * 2f - 2f) + C2) + 2f) / 2f
            };
        }

        private static float InElastic(float x)
        {
            return x switch
            {
                0f => 0f,
                1f => 1f,
                _ => -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * C4)
            };
        }

        private static float OutElastic(float x)
        {
            return x switch
            {
                0f => 0f,
                1f => 1f,
                _ => Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * C4) + 1f
            };
        }

        private static float InOutElastic(float x)
        {
            return x switch
            {
                0f => 0f,
                1f => 1f,
                < 0.5f => -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * C5)) / 2f,
                _ => (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * C5)) / 2f + 1f
            };
        }

        private static float BounceOut(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            return x switch
            {
                < 1f / d1 => n1 * x * x,
                < 2f / d1 => n1 * (x -= 1.5f / d1) * x + 0.75f,
                < 2.5f / d1 => n1 * (x -= 2.25f / d1) * x + 0.9375f,
                _ => n1 * (x -= 2.625f / d1) * x + 0.984375f
            };
        }

        private static float InBounce(float x)
        {
            return 1 - BounceOut(1 - x);
        }

        private static float OutBounce(float x)
        {
            return BounceOut(x);
        }

        private static float InOutBounce(float x)
        {
            return x switch
            {
                < 0.5f => (1f - BounceOut(1f - 2f * x)) / 2f,
                _ => (1f + BounceOut(2f * x - 1f)) / 2f
            };
        }
        
        // https://material.io/design/motion/speed.html#easing
        
        private static Vector2 Lerp2D(Vector2 a, Vector2 b, float n)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, n), Mathf.Lerp(a.y, b.y, n));
        }

        private static float CubicBezier(float _1, float _2, float _3, float _4, float n)
        {
            var a = new Vector2(0, 0);
            var b = new Vector2(_1, _2);
            var c = new Vector2(_3, _4);
            var d = new Vector2(1, 1);

            var ab = Lerp2D(a, b, n);
            var bc = Lerp2D(b, c, n);
            var cd = Lerp2D(c, d, n);
            var abbc = Lerp2D(ab, bc, n);
            var bccd = Lerp2D(bc, cd, n);
            var dest = Lerp2D(abbc, bccd, n);

            return dest.y;
        }

        private static float MaterialStandard(float n)
        {
            return CubicBezier(0.4f, 0f, 0.2f, 1f, n);
        }

        private static float MaterialDecelerate(float n)
        {
            return CubicBezier(0f, 0f, 0.2f, 1f, n);
        }

        private static float MaterialAccelerate(float n)
        {
            return CubicBezier(0.4f, 0f, 1f, 1f, n);
        }

        public static float Ease(float x, Easing easing)
        {
            return easing switch
            {
                Easing.Linear => Linear(x),
                Easing.InQuad => InQuad(x),
                Easing.OutQuad => OutQuad(x),
                Easing.InOutQuad => InOutQuad(x),
                Easing.InCubic => InCubic(x),
                Easing.OutCubic => OutCubic(x),
                Easing.InOutCubic => InOutCubic(x),
                Easing.InQuart => InQuart(x),
                Easing.OutQuart => OutQuart(x),
                Easing.InOutQuart => InOutQuart(x),
                Easing.InQuint => InQuint(x),
                Easing.OutQuint => OutQuint(x),
                Easing.InOutQuint => InOutQuint(x),
                Easing.InSine => InSine(x),
                Easing.OutSine => OutSine(x),
                Easing.InOutSine => InOutSine(x),
                Easing.InExpo => InExpo(x),
                Easing.OutExpo => OutExpo(x),
                Easing.InOutExpo => InOutExpo(x),
                Easing.InCirc => InCirc(x),
                Easing.OutCirc => OutCirc(x),
                Easing.InOutCirc => InOutCirc(x),
                Easing.InBack => InBack(x),
                Easing.OutBack => OutBack(x),
                Easing.InOutBack => InOutBack(x),
                Easing.InBounce => InBounce(x),
                Easing.OutBounce => OutBounce(x),
                Easing.InOutBounce => InOutBounce(x),
                Easing.InElastic => InElastic(x),
                Easing.OutElastic => OutElastic(x),
                Easing.InOutElastic => InOutElastic(x),
                Easing.MaterialStandard => MaterialStandard(x),
                Easing.MaterialDecelerate => MaterialDecelerate(x),
                Easing.MaterialAccelerate => MaterialAccelerate(x),
                _ => throw new NotImplementedException("Easing not implemented: " + easing)
            };
        }
    }
}