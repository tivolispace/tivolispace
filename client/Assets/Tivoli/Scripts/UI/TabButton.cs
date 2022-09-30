using Tivoli.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private float _closedWidth;
    private float _openWidth;

    private const float ClosedHeight = 24;
    private const float OpenHeight = 32;

    private const float ClosedFontSize = 20;
    private const float OpenFontSize = 22;
    
    private float _closedFontSpacing;
    private float _openFontSpacing;

    private bool _open;

    private TweenManager _tweenManager;

    private TweenManager.Tweener _width;
    private TweenManager.Tweener _height;
    private TweenManager.Tweener _fontSize;
    private TweenManager.Tweener _fontSpacing;
    
    void Awake()
    {
        _tweenManager = new TweenManager();

        var rectTransform = GetComponent<RectTransform>();
        var text = GetComponentInChildren<TextMeshProUGUI>();

        _closedWidth = rectTransform.sizeDelta.x;
        _openWidth = rectTransform.sizeDelta.x + 12f;

        _closedFontSpacing = text.characterSpacing;
        _openFontSpacing = 0f;
        
        _width = _tweenManager.NewTweener(width =>
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }, _closedWidth);
        
        _height = _tweenManager.NewTweener(height =>
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }, ClosedHeight);
        
        _fontSize = _tweenManager.NewTweener(fontSize =>
        {
            text.fontSize = fontSize;
        }, ClosedFontSize);
        
        _fontSpacing = _tweenManager.NewTweener(fontSpacing =>
        {
            text.characterSpacing = fontSpacing;
        }, _closedFontSpacing);
    }

    void Update()
    {
        _tweenManager.Update();
    }

    private void ShowOpen()
    {
        _width.Tween(_openWidth, 200, EasingFunctions.Easing.OutQuint);
        _height.Tween(OpenHeight, 300, EasingFunctions.Easing.OutQuint);
        _fontSize.Tween(OpenFontSize, 400, EasingFunctions.Easing.OutQuint);
        _fontSpacing.Tween(_openFontSpacing, 400, EasingFunctions.Easing.OutQuint);
    }

    private void ShowClosed()
    {
        _width.Tween(_closedWidth, 300, EasingFunctions.Easing.OutQuint);
        _height.Tween(ClosedHeight, 450, EasingFunctions.Easing.OutQuint);
        _fontSize.Tween(ClosedFontSize, 600, EasingFunctions.Easing.OutQuint); 
        _fontSpacing.Tween(_closedFontSpacing, 600, EasingFunctions.Easing.OutQuint); 
    }

    public void SetOpen(bool open)
    {
        _open = open;
        if (_open)
        {
            ShowOpen();
        }
        else
        {
            ShowClosed();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_open) ShowOpen();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_open) ShowClosed();
    }
}
