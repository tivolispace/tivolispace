using System;
using System.Collections;
using System.Collections.Generic;
using Tivoli.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ButtonHoverTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private TextMeshProUGUI _text;
    
    private readonly TweenManager _tweenManager = new();

    private float _initialButtonWidth;
    private TweenManager.Tweener _buttonWidth;

    private float _initialCharacterSpacing;
    private TweenManager.Tweener _characterSpacing;

    private const float HoverDuration = 250; // ms
    private const EasingFunctions.Easing Easing = EasingFunctions.Easing.OutQuint;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        _initialButtonWidth = _rectTransform.sizeDelta.x;
        _buttonWidth =
            _tweenManager.NewTweener(
                width => { _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y); },
                _initialButtonWidth
            );

        _initialCharacterSpacing = _text.characterSpacing;
        _characterSpacing =
            _tweenManager.NewTweener(spacing => { _text.characterSpacing = spacing; }, _initialCharacterSpacing);
    }

    private void Update()
    {
        _tweenManager.Update();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _buttonWidth.Tween(_initialButtonWidth + 24f, HoverDuration, Easing);
        _characterSpacing.Tween(4f, HoverDuration, Easing);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonWidth.Tween(_initialButtonWidth, HoverDuration, Easing);
        _characterSpacing.Tween(_initialCharacterSpacing, HoverDuration, Easing);
    }
}