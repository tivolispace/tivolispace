using System;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager
{
    public enum Interpolation
    {
        Standard,
        Decelerate,
        Accelerate,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
    }

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

    // https://material.io/design/motion/speed.html#easing

    private static float StandardEasing(float n)
    {
        return CubicBezier(0.4f, 0f, 0.2f, 1f, n);
    }

    private static float DecelerateEasing(float n)
    {
        return CubicBezier(0f, 0f, 0.2f, 1f, n);
    }

    private static float AccelerateEasing(float n)
    {
        return CubicBezier(0.4f, 0f, 1f, 1f, n);
    }

    // https://github.com/ai/easings.net/blob/33774b5880a787e467d6f4f65000608d17b577e2/src/easings/easingsFunctions.ts

    private const float c4 = (2f * Mathf.PI) / 3f;
    private const float c5 = (2f * Mathf.PI) / 4.5f;

    private static float EaseInElastic(float x)
    {
        return x switch
        {
            0f => 0f,
            1f => 1f,
            _ => -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4)
        };
    }

    private static float EaseOutElastic(float x)
    {
        return x switch
        {
            0f => 0f,
            1f => 1f,
            _ => Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f
        };
    }

    private static float EaseInOutElastic(float x)
    {
        return x switch
        {
            0f => 0f,
            1f => 1f,
            < 0.5f => -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f,
            _ => (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f + 1f
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

    private static float EaseInBounce(float x)
    {
        return 1 - BounceOut(1 - x);
    }

    private static float EaseOutBounce(float x)
    {
        return BounceOut(x);
    }

    private static float EaseInOutBounce(float x)
    {
        return x switch
        {
            < 0.5f => (1f - BounceOut(1f - 2f * x)) / 2f,
            _ => (1f + BounceOut(2f * x - 1f)) / 2f
        };
    }

    public class Tweener
    {
        public float From;
        public float To;

        private float _startTime;
        private float _endTime;

        public float Current;
        public bool Finished = true;

        private Interpolation _interpolation;
        private readonly Action<float> _transition;

        public Tweener(Action<float> transition, float initial = 0)
        {
            _transition = transition;
            From = initial;
            To = initial;
            Current = initial;
            transition(initial);
        }

        public void Tween(float to, float ms, Interpolation interpolation = Interpolation.Standard)
        {
            From = Current;
            To = to;

            _startTime = Time.time;
            _endTime = Time.time + ms / 1000;

            _interpolation = interpolation;
            Finished = false;
        }

        public void Update()
        {
            if (Finished) return;

            if (Time.time > _endTime)
            {
                _transition(To);
                Current = To;
                Finished = true;
                return;
            }

            var duration = _endTime - _startTime;

            var t = (Time.time - _startTime) / duration;
            t = _interpolation switch
            {
                Interpolation.Standard => StandardEasing(t),
                Interpolation.Accelerate => AccelerateEasing(t),
                Interpolation.Decelerate => DecelerateEasing(t),
                Interpolation.EaseInElastic => EaseInElastic(t),
                Interpolation.EaseOutElastic => EaseOutElastic(t),
                Interpolation.EaseInOutElastic => EaseInOutElastic(t),
                Interpolation.EaseInBounce => EaseInBounce(t),
                Interpolation.EaseOutBounce => EaseOutBounce(t),
                Interpolation.EaseInOutBounce => EaseInOutBounce(t),
                _ => t
            };

            var n = Mathf.Lerp(From, To, t);

            _transition(n);
            Current = n;
        }
    }

    private List<Tweener> _tweeners = new();

    public Tweener NewTweener(Action<float> transition, float initial = 0)
    {
        var tweener = new Tweener(transition, initial);
        _tweeners.Add(tweener);
        return tweener;
    }

    public void Update()
    {
        foreach (var tweener in _tweeners)
        {
            tweener.Update();
        }
    }
}