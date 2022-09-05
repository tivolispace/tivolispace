using System;
using System.Collections.Generic;
using Tivoli.Scripts.UI;
using UnityEngine;

public class TweenManager
{
    public class Tweener
    {
        public float From;
        public float To;

        private float _startTime;
        private float _endTime;

        public float Current;
        public bool Finished = true;

        private EasingFunctions.Easing _easingFunction;
        private readonly Action<float> _transitionFunction;

        public Tweener(Action<float> transitionFunction, float initial = 0)
        {
            _transitionFunction = transitionFunction;
            From = initial;
            To = initial;
            Current = initial;
            transitionFunction(initial);
        }

        public void Tween(float to, float ms, EasingFunctions.Easing easingFunction)
        {
            From = Current;
            To = to;

            _startTime = Time.time;
            _endTime = Time.time + ms / 1000;

            _easingFunction = easingFunction;
            Finished = false;
        }

        public void Update()
        {
            if (Finished) return;

            if (Time.time > _endTime)
            {
                _transitionFunction(To);
                Current = To;
                Finished = true;
                return;
            }

            var duration = _endTime - _startTime;

            var t = (Time.time - _startTime) / duration;
            t = EasingFunctions.Ease(t, _easingFunction);

            var n = Mathf.Lerp(From, To, t);

            _transitionFunction(n);
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