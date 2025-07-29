using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class UICardMover : MonoBehaviour
{
    private RectTransform rectTransform;

    public void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveTo(Vector2 targetPos, float duration, Ease easeType, float delay = 0f, TweenCallback onComplete = null)
    {
        rectTransform.DOAnchorPos(targetPos, duration)
            .SetDelay(delay)
            .SetEase(easeType)
            .OnComplete(onComplete);
    }

    public Vector2 GetPosition()
    {
        return rectTransform.anchoredPosition;
    }
}
