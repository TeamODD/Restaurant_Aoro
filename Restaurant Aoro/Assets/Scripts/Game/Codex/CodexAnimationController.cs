using System.Collections;
using UnityEngine;
using System;

public class CodexAnimationController : MonoBehaviour
{
    [Header("Codex")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject detailPanel;

    [Header("Components")]
    [SerializeField] private CanvasGroup infoCanvasGroup;
    [SerializeField] private CanvasGroup detailCanvasGroup;
    [SerializeField] private RectTransform detailRectTransform;
    [Header("Moving Objects")]
    [SerializeField] private RectTransform movingRootRectTransform;
    [SerializeField] private float movingOffsetX = 500f;

    [Header("Animation Settings")]
    [SerializeField] private float infoFadeAlpha = 0.25f;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float detailStartOffsetX = -1000f;

    private Vector2 detailOriginPosition;
    private Vector2 movingRootOriginPosition;
    private Coroutine animationCoroutine;
    private bool isTransitioning;
    public bool IsTransitioning => isTransitioning;

    private void Awake()
    {
        if (infoCanvasGroup == null && infoPanel != null)
            infoCanvasGroup = infoPanel.GetComponent<CanvasGroup>();

        if (detailCanvasGroup == null && detailPanel != null)
            detailCanvasGroup = detailPanel.GetComponent<CanvasGroup>();

        if (detailRectTransform == null && detailPanel != null)
            detailRectTransform = detailPanel.GetComponent<RectTransform>();

        if (detailRectTransform != null)
            detailOriginPosition = detailRectTransform.anchoredPosition;

        if (movingRootRectTransform != null)
            movingRootOriginPosition = movingRootRectTransform.anchoredPosition;
    }

    public void PlayInfoToDetail(Action onCompleted = null)
    {
        if (isTransitioning)
            return;

        StartAnimation(InfoToDetailRoutine(onCompleted));
    }

    public void PlayDetailToInfo(Action onCompleted = null)
    {
        if (isTransitioning)
            return;

        StartAnimation(DetailToInfoRoutine(onCompleted));
    }

    public void ResetToInfo()
    {
        StopAnimation();

        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (detailPanel != null)
            detailPanel.SetActive(false);

        if (infoCanvasGroup != null)
        {
            infoCanvasGroup.alpha = 1f;
            SetInteraction(infoCanvasGroup, true);
        }

        if (detailCanvasGroup != null)
        {
            detailCanvasGroup.alpha = 1f;
            SetInteraction(detailCanvasGroup, false);
        }

        if (detailRectTransform != null)
            detailRectTransform.anchoredPosition = detailOriginPosition;

        if (movingRootRectTransform != null)
        {
            movingRootRectTransform.anchoredPosition =
                movingRootOriginPosition;
        }
    }

    private void StartAnimation(IEnumerator routine)
    {
        StopAnimation();
        animationCoroutine = StartCoroutine(routine);
    }

    private IEnumerator InfoToDetailRoutine(Action onCompleted)
    {
        if (!HasValidReferences())
            yield break;

        isTransitioning = true;

        infoPanel.SetActive(true);
        detailPanel.SetActive(true);

        SetInteraction(infoCanvasGroup, false);
        SetInteraction(detailCanvasGroup, false);

        Vector2 detailStartPosition =
            detailOriginPosition +
            Vector2.right * detailStartOffsetX;

        detailRectTransform.anchoredPosition =
            detailStartPosition;

        float elapsedTime = 0f;
        float startInfoAlpha = infoCanvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            infoCanvasGroup.alpha = Mathf.Lerp(
                startInfoAlpha,
                0f,
                ratio
            );

            yield return null;
        }

        infoCanvasGroup.alpha = 0f;
        infoPanel.SetActive(false);

        Vector2 movingRootDetailPosition =
            movingRootOriginPosition +
            Vector2.right * movingOffsetX;

        elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / slideDuration
            );

            float easedRatio = EaseOutCubic(ratio);

            detailRectTransform.anchoredPosition =
                Vector2.Lerp(
                    detailStartPosition,
                    detailOriginPosition,
                    easedRatio
                );

            movingRootRectTransform.anchoredPosition =
                Vector2.Lerp(
                    movingRootOriginPosition,
                    movingRootDetailPosition,
                    easedRatio
                );

            yield return null;
        }

        detailRectTransform.anchoredPosition =
            detailOriginPosition;

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            yield return null;
        }

        SetInteraction(detailCanvasGroup, true);

        FinishAnimation();

        onCompleted?.Invoke();
    }

    private IEnumerator DetailToInfoRoutine(Action onCompleted)
    {
        if (!HasValidReferences())
            yield break;

        isTransitioning = true;

        infoPanel.SetActive(true);
        detailPanel.SetActive(true);

        SetInteraction(infoCanvasGroup, false);
        SetInteraction(detailCanvasGroup, false);

        infoCanvasGroup.alpha = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            yield return null;
        }
        Vector2 movingRootDetailPosition =
            movingRootOriginPosition +
            Vector2.right * movingOffsetX;

        Vector2 detailEndPosition =
            detailOriginPosition +
            Vector2.right * detailStartOffsetX;

        Vector2 detailStartPosition =
            detailRectTransform.anchoredPosition;

        elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / slideDuration
            );

            float easedRatio = EaseInCubic(ratio);

            detailRectTransform.anchoredPosition =
                Vector2.Lerp(
                    detailStartPosition,
                    detailEndPosition,
                    easedRatio
                );

            movingRootRectTransform.anchoredPosition =
                Vector2.Lerp(
                    movingRootDetailPosition,
                    movingRootOriginPosition,
                    easedRatio
                );

            yield return null;
        }

        detailRectTransform.anchoredPosition =
            detailEndPosition;

        detailPanel.SetActive(false);

        detailRectTransform.anchoredPosition =
            detailOriginPosition;

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            infoCanvasGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                ratio
            );

            yield return null;
        }

        infoCanvasGroup.alpha = 1f;

        SetInteraction(infoCanvasGroup, true);

        FinishAnimation();

        onCompleted?.Invoke();
    }

    private bool HasValidReferences()
    {
        if (infoPanel == null ||
            detailPanel == null ||
            infoCanvasGroup == null ||
            detailCanvasGroup == null ||
            detailRectTransform == null ||
            movingRootRectTransform == null)
        {
            Debug.LogWarning(
                "[CodexAnimatorController] 필요한 UI 참조가 연결되지 않았습니다."
            );

            FinishAnimation();
            return false;
        }

        return true;
    }
    private IEnumerator MoveRectTransform(
        RectTransform target,
        Vector2 startPosition,
        Vector2 endPosition,
        float duration,
        bool useEaseOut)
    {
        if (target == null)
            yield break;

        if (duration <= 0f)
        {
            target.anchoredPosition = endPosition;
            yield break;
        }

        target.anchoredPosition = startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(
                elapsedTime / duration
            );

            float easedRatio = useEaseOut
                ? EaseOutCubic(ratio)
                : EaseInCubic(ratio);

            target.anchoredPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                easedRatio
            );

            yield return null;
        }

        target.anchoredPosition = endPosition;
    }

    private void SetInteraction(
        CanvasGroup canvasGroup,
        bool interactable)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    private void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        isTransitioning = false;
    }

    private void FinishAnimation()
    {
        animationCoroutine = null;
        isTransitioning = false;
    }

    private float EaseOutCubic(float value)
    {
        return 1f - Mathf.Pow(1f - value, 3f);
    }

    private float EaseInCubic(float value)
    {
        return value * value * value;
    }
}