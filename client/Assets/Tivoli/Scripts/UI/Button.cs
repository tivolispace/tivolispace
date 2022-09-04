using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Button : MonoBehaviour
{
    public TextMeshProUGUI text;

    private readonly TweenManager _tweenManager = new();

    private float _initialCubeWidth;
    private TweenManager.Tweener _cubeWidth;

    private TweenManager.Tweener _characterSpacing;

    private const float HoverDuration = 250; // ms
    private const TweenManager.Interpolation Interpolation = TweenManager.Interpolation.EaseOutBounce;

    private void Awake()
    {
        _initialCubeWidth = transform.localScale.x;
        _cubeWidth =
            _tweenManager.NewTweener(
                width => { transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z); },
                _initialCubeWidth
            );

        _characterSpacing = _tweenManager.NewTweener(spacing => { text.characterSpacing = spacing; }, 0f);
    }

    private void Update()
    {
        _tweenManager.Update();
    }

    private void OnMouseEnter()
    {
        _cubeWidth.Tween(_initialCubeWidth + 1f, HoverDuration, Interpolation);
        _characterSpacing.Tween(text.fontSize, HoverDuration, Interpolation);
    }

    private void OnMouseExit()
    {
        _cubeWidth.Tween(_initialCubeWidth, HoverDuration, Interpolation);
        _characterSpacing.Tween(0, HoverDuration, Interpolation);
    }
}