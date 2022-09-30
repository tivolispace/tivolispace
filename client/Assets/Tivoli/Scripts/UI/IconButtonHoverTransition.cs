using Tivoli.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconButtonHoverTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    
    private readonly TweenManager _tweenManager = new();

    private float _initialButtonSize;
    private TweenManager.Tweener _buttonSize;

    private const float HoverDuration = 250; // ms
    private const EasingFunctions.Easing Easing = EasingFunctions.Easing.OutQuint;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        // should be same size
        _initialButtonSize = (_rectTransform.sizeDelta.x + _rectTransform.sizeDelta.y) / 2f;
        _buttonSize =
            _tweenManager.NewTweener(
                size => { _rectTransform.sizeDelta = new Vector2(size, size); },
                _initialButtonSize
            );
    }

    private void Update()
    {
        _tweenManager.Update();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _buttonSize.Tween(_initialButtonSize * 1.1f, HoverDuration, Easing);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonSize.Tween(_initialButtonSize, HoverDuration, Easing);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _buttonSize.Tween(_initialButtonSize * 0.9f, HoverDuration, Easing);
    }
}