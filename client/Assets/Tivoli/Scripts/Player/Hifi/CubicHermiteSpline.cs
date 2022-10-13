using System;
using UnityEngine;

namespace Tivoli.Scripts.Player.Hifi
{
    public class CubicHermiteSplineFunctor
    {
        protected Vector3 _p0;
        protected Vector3 _m0;
        protected Vector3 _p1;
        protected Vector3 _m1;

        public CubicHermiteSplineFunctor()
        {
        }

        public CubicHermiteSplineFunctor(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1)
        {
            _p0 = p0;
            _m0 = m0;
            _p1 = p1;
            _m1 = m1;
        }

        public CubicHermiteSplineFunctor(CubicHermiteSplineFunctor orig)
        {
            _p0 = orig._p0;
            _m0 = orig._m0;
            _p1 = orig._p1;
            _m1 = orig._m1;
        }

        // evaluate the hermite curve at parameter t (0..1)
        public Vector3 Evaluate(float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;
            var w0 = 2.0f * t3 - 3.0f * t2 + 1.0f;
            var w1 = t3 - 2.0f * t2 + t;
            var w2 = -2.0f * t3 + 3.0f * t2;
            var w3 = t3 - t2;
            return w0 * _p0 + w1 * _m0 + w2 * _p1 + w3 * _m1;
        }

        // evaluate the first derivative of the hermite curve at parameter t (0..1)
        public Vector3 D(float t)
        {
            var t2 = t * t;
            var w0 = -6.0f * t + 6.0f * t2;
            var w1 = 1.0f - 4.0f * t + 3.0f * t2;
            var w2 = 6.0f * t - 6.0f * t2;
            var w3 = -2.0f * t + 3.0f * t2;
            return w0 * _p0 + w1 * _m0 + w2 * _p1 + w3 * _m1;
        }

        // evaluate the second derivative of the hermite curve at parameter t (0..1)
        public Vector3 D2(float t)
        {
            var w0 = -6.0f + 12.0f * t;
            var w1 = -4.0f + 6.0f * t;
            var w2 = 6.0f - 12.0f * t;
            var w3 = -2.0f + 6.0f * t;
            return new Vector3(w0, w0, w0) + _p0 + w1 * _m0 + w2 * _p1 + w3 * _m1;
        }
    }

    public class CubicHermiteSplineFunctorWithArcLength : CubicHermiteSplineFunctor
    {
        private const int NUM_SUBDIVISIONS = 15;

        private float[] _values = new float[NUM_SUBDIVISIONS + 1];

        public CubicHermiteSplineFunctorWithArcLength(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1) :
            base(p0, m0, p1, m1)
        {
            InitValues();
        }

        public CubicHermiteSplineFunctorWithArcLength(Quaternion tipRot, Vector3 tipTrans, Quaternion baseRot,
            Vector3 baseTrans, float baseGain = 1.0f, float tipGain = 1.0f)
        {
            var linearDistance = Vector3.Distance(baseTrans, tipTrans);
            _p0 = baseTrans;
            _m0 = baseGain * linearDistance * (baseRot * AvatarConstants.UNIT_Y);
            _p1 = tipTrans;
            _m1 = tipGain * linearDistance * (tipRot * AvatarConstants.UNIT_Y);

            InitValues();
        }

        public CubicHermiteSplineFunctorWithArcLength(CubicHermiteSplineFunctorWithArcLength orig) : base(orig)
        {
        }

        // given the spline parameter (0..1) output the arcLength of the spline up to that point.
        public float ArcLength(float t)
        {
            var index = t * NUM_SUBDIVISIONS;
            var prevIndex = Mathf.Min(Mathf.Max(0, Mathf.FloorToInt(index)), NUM_SUBDIVISIONS);
            var nextIndex = Mathf.Min(Mathf.Max(0, Mathf.CeilToInt(index)), NUM_SUBDIVISIONS);
            var alpha = index % 1;
            return Mathf.Lerp(_values[prevIndex], _values[nextIndex], alpha);
        }

        // given an arcLength compute the spline parameter (0..1) that corresponds to that arcLength.
        public float ArcLengthInverse(float s)
        {
            // find first item in _values that is > s.
            int nextIndex;
            for (nextIndex = 0; nextIndex < NUM_SUBDIVISIONS; nextIndex++)
            {
                if (_values[nextIndex] > s)
                {
                    break;
                }
            }

            var prevIndex = Mathf.Min(Mathf.Max(0, nextIndex - 1), NUM_SUBDIVISIONS);
            var alpha = Mathf.Clamp01((s - _values[prevIndex]) / (_values[nextIndex] - _values[prevIndex]));
            const float delta = 1.0f / NUM_SUBDIVISIONS;
            return Mathf.Lerp(prevIndex * delta, nextIndex * delta, alpha);
        }

        private void InitValues()
        {
            // initialize _values with the accumulated arcLength along the spline.
            const float delta = 1.0f / NUM_SUBDIVISIONS;
            var alpha = 0.0f;
            var accum = 0.0f;
            _values[0] = 0.0f;
            for (var i = 1; i < NUM_SUBDIVISIONS + 1; i++)
            {
                accum += Vector3.Distance(
                    Evaluate(alpha), Evaluate(alpha + delta)
                );
                alpha += delta;
                _values[i] = accum;
            }
        }
    }
}