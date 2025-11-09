using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DialogueInputMode { Global, Blocker }

public class DialogueUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform panel;          // 말풍선 배경(RectTransform)
    [SerializeField] private TextMeshProUGUI text;         // TMP 텍스트
    [SerializeField] private CanvasGroup group;            // 페이드
    [SerializeField] private Camera cam;

    [Header("Options")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 80f);  // 말풍선을 앵커 위로 띄우는 픽셀 오프셋
    [SerializeField] private float fadeTime = 0.15f;                       // 페이드 시간
    [SerializeField] private float charInterval = 0.2f;                   // 타자기 간격(초당 50자 0.02)
    [SerializeField] private bool clampToScreen = true;                    // 화면 밖으로 나가지 않기
    [SerializeField] private Vector2 padding = new Vector2(320f, 180f);      // 텍스트 밖 여백(px)
    [SerializeField] private float maxWidth = 650f;                        // 말풍선 최대 너비(px), 자동 줄바꿈 기반

    [SerializeField] private float minBubbleWidth = 140f;
    [SerializeField] private float minBubbleHeight = 64f;

    [Header("Blocker")]
    [SerializeField] private GameObject clickBlocker;
    [SerializeField] private bool closeOnBlockerClick = true;
    [SerializeField] private bool useInternalClick = false;

    [Header("Tail")]
    [SerializeField] private RectTransform tail;        // 꼬리 이미지
    [SerializeField] private RectTransform tailAnchor;  // 새로 만든 앵커 (패널 자식)

    [SerializeField, Range(0f, 1f)] private float tailAnchorX = 0.28f; // 0=왼쪽, 1=오른쪽
    [SerializeField, Range(-0.5f, 1f)] private float tailAnchorY = -0.09f;
    // 크기/스케일 옵션
    [SerializeField] private Vector2 tailBaseSize = new Vector2(64, 48);          // 기준 꼬리 크기
    [SerializeField] private Vector2 tailBasePanelSize = new Vector2(1000, 200);   // 기준 패널 크기
    [SerializeField] private bool tailUniformScale = true;
    [SerializeField] private float tailMinScale = 0.5f;
    [SerializeField] private float tailMaxScale = 1.75f;

    [SerializeField] private DialogueInputMode defaultInputMode = DialogueInputMode.Blocker;
    private DialogueInputMode currentMode;
    public DialogueInputMode CurrentMode => currentMode;

    // 내부 상태
    private Transform worldAnchor;
    private Coroutine followRoutine;
    private Coroutine typeRoutine;
    private Coroutine fadeRoutine;

    private readonly Queue<string> lineQueue = new Queue<string>();
    private string currentLine = "";
    private bool isTyping = false;
    private bool isShowing = false;

    private void Awake()
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        if (panel) FixCenterAnchor(panel);
        if (text) FixCenterAnchor(text.rectTransform);

        HideImmediate();

        if (clickBlocker != null)
        {
            var blk = clickBlocker.GetComponent<DialogueClickBlocker>();
            if (blk == null) blk = clickBlocker.AddComponent<DialogueClickBlocker>();
            blk.target = this;

            clickBlocker.SetActive(false);

            var rt = clickBlocker.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }
    }

    public void AdvanceOrComplete()
    {
        if (!isShowing) return;

        if (isTyping)
        {
            ForceCompleteTyping();
            return;
        }

        if (lineQueue.Count > 0)
        {
            ShowNextInternal(worldAnchor);
            return;
        }

        StartCoroutine(FadeOutAndHide());
    }

    private void EnableBlocker(bool on)
    {
        if (clickBlocker == null) return;
        clickBlocker.SetActive(on);

        if (on)
            clickBlocker.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void UpdateTailSize()
    {
        if (tail == null || panel == null) return;

        var p = panel.sizeDelta;
        float sx = p.x / Mathf.Max(1f, tailBasePanelSize.x);
        float sy = p.y / Mathf.Max(1f, tailBasePanelSize.y);

        if (tailUniformScale)
        {
            float s = Mathf.Clamp(Mathf.Min(sx, sy), tailMinScale, tailMaxScale);
            tail.sizeDelta = tailBaseSize * s;
        }
        else
        {
            sx = Mathf.Clamp(sx, tailMinScale, tailMaxScale);
            sy = Mathf.Clamp(sy, tailMinScale, tailMaxScale);
            tail.sizeDelta = new Vector2(tailBaseSize.x * sx, tailBaseSize.y * sy);
        }
    }

    private static void FixCenterAnchor(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private void EnsureActive()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;
        if (group != null) group.blocksRaycasts = true;

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
                    cam = canvas.worldCamera;
            }
        }
    }



    public void ShowLines(IEnumerable<string> lines, Transform anchor, float holdLastSeconds = 0f, DialogueInputMode? mode = null)
    {
        EnsureActive();
        ClearQueue();

        if (lines != null)
            foreach (var s in lines)
                if (!string.IsNullOrWhiteSpace(s)) lineQueue.Enqueue(s.Trim());

        currentMode = mode ?? defaultInputMode;
        ShowNextInternal(anchor, holdLastSeconds);
    }

    public void ShowOne(string line, Transform anchor, float holdSeconds = 1.6f, DialogueInputMode? mode = null)
    {
        EnsureActive();
        ClearQueue();

        if (!string.IsNullOrWhiteSpace(line))
            lineQueue.Enqueue(line.Trim());

        currentMode = mode ?? defaultInputMode;
        ShowNextInternal(anchor, holdSeconds);
    }

    public void HideImmediate()
    {
        if (followRoutine != null) StopCoroutine(followRoutine);
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        isTyping = false;
        isShowing = false;
        currentLine = "";
        lineQueue.Clear();

        group.alpha = 0f;
        EnableBlocker(false);
        gameObject.SetActive(false);
    }

    public bool IsShowing() => isShowing;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isShowing) return;

        if (isTyping)
        {
            ForceCompleteTyping();
            return;
        }

        if (lineQueue.Count > 0)
        {
            ShowNextInternal(worldAnchor);
            return;
        }

        StartCoroutine(FadeOutAndHide());

        if (!useInternalClick) return;
        AdvanceOrComplete();
    }

    private void ShowNextInternal(Transform anchor, float holdLastSeconds = 0f)
    {
        EnsureActive();
        worldAnchor = anchor;  

        if (lineQueue.Count == 0)
        {
            if (holdLastSeconds > 0f) { StartCoroutine(ShowAndHideLast(holdLastSeconds)); }
            else { StartCoroutine(FadeOutAndHide()); }
            return;
        }

        currentLine = lineQueue.Dequeue();

        gameObject.SetActive(true);
        isShowing = true;

        if (currentMode == DialogueInputMode.Blocker)
            EnableBlocker(true);  

        if (followRoutine != null) StopCoroutine(followRoutine);
        followRoutine = StartCoroutine(FollowAnchor()); 

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(1f, fadeTime));

        StartTyping(currentLine);

    }

    private void ShowNextInternal(Transform anchor)
    {
        ShowNextInternal(anchor, 0f);
    }

    private IEnumerator ShowAndHideLast(float holdSeconds)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            isShowing = true;
        }
        if (currentMode == DialogueInputMode.Blocker)
            EnableBlocker(true);

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(1f, fadeTime));

        yield return new WaitForSeconds(holdSeconds);
        yield return FadeOutAndHide();
    }

    private void StartTyping(string line)
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);

        text.enableAutoSizing = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;
        text.margin = Vector4.zero;

        text.text = line;

        RebuildAndResizePanel();

        typeRoutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        isTyping = true;

        text.ForceMeshUpdate();
        int total = text.textInfo.characterCount;
        if (total == 0)
        {
            isTyping = false;
            yield break;
        }

        int visible = 0;
        text.maxVisibleCharacters = 0;

        while (visible < total)
        {
            visible++;
            text.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(charInterval);
        }

        isTyping = false;
        text.maxVisibleCharacters = total;

        RebuildAndResizePanel();
    }

    private void ForceCompleteTyping()
    {
        if (!isTyping) return;

        text.ForceMeshUpdate();
        text.maxVisibleCharacters = text.textInfo.characterCount;

        isTyping = false;
        RebuildAndResizePanel();
    }

    private void RebuildAndResizePanel()
    {
        text.ForceMeshUpdate();

        Vector2 pref = text.GetPreferredValues(text.text, maxWidth, Mathf.Infinity);
        float textW = Mathf.Min(pref.x, maxWidth);
        float textH = pref.y;

        var rt = text.rectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textW);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textH);

        float bubbleW = Mathf.Max(minBubbleWidth, textW + padding.x * 2f);
        float bubbleH = Mathf.Max(minBubbleHeight, textH + padding.y * 2f);
        panel.sizeDelta = new Vector2(bubbleW, bubbleH);

        if (tail != null)
        {
            float tx = (bubbleW * tailAnchorX) - (bubbleW * 0.5f);
            float ty = (bubbleH * tailAnchorY) - (bubbleH * 0.5f);
            tail.anchoredPosition = new Vector2(tx, ty);

            // 스케일 조정 (선택적)
            float scaleFactor = bubbleW / tailBasePanelSize.x;
            if (tailUniformScale)
                tail.localScale = Vector3.one * Mathf.Clamp(scaleFactor, tailMinScale, tailMaxScale);
            else
                tail.localScale = new Vector3(
                    Mathf.Clamp(scaleFactor, tailMinScale, tailMaxScale),
                    Mathf.Clamp(bubbleH / tailBasePanelSize.y, tailMinScale, tailMaxScale),
                    1f
                );
        }
    }

    private IEnumerator FollowAnchor()
    {

        var canvas = GetComponentInParent<Canvas>();
        RectTransform container = panel != null ? panel.parent as RectTransform : null;

        Camera camForRT = null;
        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camForRT = canvas.worldCamera;      
            else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                camForRT = null;                
            else
                camForRT = cam != null ? cam : Camera.main; 
        }
        else
        {
            camForRT = cam != null ? cam : Camera.main;
        }

        while (true)
        {
            if (worldAnchor != null)
            {
                Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(camForRT ?? cam, worldAnchor.position);

                if (container != null)
                {
                    Vector2 localPt;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPt, camForRT, out localPt);

                    Vector2 target = localPt + screenOffset;

                    if (clampToScreen)
                    {
                        float halfCW = container.rect.width * 0.5f;
                        float halfCH = container.rect.height * 0.5f;

                        float halfPW = panel.sizeDelta.x * 0.5f;
                        float halfPH = panel.sizeDelta.y * 0.5f;

                        target.x = Mathf.Clamp(target.x, -halfCW + halfPW, halfCW - halfPW);
                        target.y = Mathf.Clamp(target.y, -halfCH + halfPH, halfCH - halfPH);
                    }
                    panel.anchoredPosition = target;
                }
                else
                {
                    panel.position = screenPt + screenOffset;
                }
            }

            yield return null;
        }
    }

    private IEnumerator FadeOutAndHide()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        yield return FadeTo(0f, fadeTime);

        // 마무리
        if (followRoutine != null) StopCoroutine(followRoutine);
        isTyping = false;
        isShowing = false;

        if (group != null) group.blocksRaycasts = false;
        EnableBlocker(false);
        gameObject.SetActive(false);     
    }

    public void OnBlockerClicked()
    {
        if (!closeOnBlockerClick || !isShowing) return;

        if (isTyping)
        {
            ForceCompleteTyping();
            return;
        }

        if (lineQueue.Count > 0)
        {
            ShowNextInternal(worldAnchor);
            return;
        }

        StartCoroutine(FadeOutAndHide());
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(duration, 0.0001f);
            group.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
        group.alpha = target;
    }

    private void ClearQueue()
    {
        lineQueue.Clear();
        currentLine = "";
    }
}
