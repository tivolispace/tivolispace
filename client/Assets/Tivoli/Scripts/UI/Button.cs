using System;
using System.Collections;
using System.Collections.Generic;
using Tivoli.Scripts.UI;
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
    private const EasingFunctions.Easing Easing = EasingFunctions.Easing.OutQuint;

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
        _cubeWidth.Tween(_initialCubeWidth + 1f, HoverDuration, Easing);
        _characterSpacing.Tween(text.fontSize, HoverDuration, Easing);
    }

    private void OnMouseExit()
    {
        _cubeWidth.Tween(_initialCubeWidth, HoverDuration, Easing);
        _characterSpacing.Tween(0, HoverDuration, Easing);
    }
}