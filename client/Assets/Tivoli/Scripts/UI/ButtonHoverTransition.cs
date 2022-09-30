using System;
using System.Collections;
using System.Collections.Generic;
using Tivoli.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ButtonHoverTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    private TextMeshProUGUI _text;

    private readonly TweenManager _tweenManager = new();

    private float _initialButtonScale;
    private TweenManager.Tweener _buttonScale;

    private float _initialCharacterSpacing;
    private TweenManager.Tweener _characterSpacing;

    private const float HoverDuration = 250; // ms
    private const EasingFunctions.Easing Easing = EasingFunctions.Easing.OutQuint;

    public bool scaleVertically;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        _initialButtonScale = scaleVertically ? _rectTransform.sizeDelta.y : _rectTransform.sizeDelta.x;
        _buttonScale =
            _tweenManager.NewTweener(
                scale =>
                {
                    _rectTransform.sizeDelta = scaleVertically
                        ? new Vector2(_rectTransform.sizeDelta.x, scale)
                        : new Vector2(scale, _rectTransform.sizeDelta.y);
                },
                _initialButtonScale
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
        _buttonScale.Tween(_initialButtonScale * 1.2f, HoverDuration, Easing);
        _characterSpacing.Tween(4f, HoverDuration, Easing);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonScale.Tween(_initialButtonScale, HoverDuration, Easing);
        _characterSpacing.Tween(_initialCharacterSpacing, HoverDuration, Easing);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _buttonScale.Tween(_initialButtonScale * 0.9f, HoverDuration, Easing);
        _characterSpacing.Tween(_initialCharacterSpacing - 2f, HoverDuration, Easing);
    }
}