using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public InventoryManager invmanager;
    public bool IsInventoryOpen => isArrowCentered;
    private RectTransform[] panels;
    private Vector2 currentOffset;
    private RectTransform rightArrow { get; set; }
    private Vector2 rightArrowInitialPos;
    private bool isArrowCentered = false;

    private Coroutine moveCoroutine;
    private Coroutine focusRoutine;
    private Vector3 originalCamPos;

    private HashSet<CanvasGroup> _lastOpaqueArrows;

    public Vector2 BaseOffset { get; private set; }
    public Vector2 CurrentOffset => currentOffset;
    public bool IsAnimating { get; private set; }

    public void SetIsAnimating(bool value) => IsAnimating = value;

    private static event Action OffServeBtn;

    public void Initialize(RectTransform[] panelRects, Vector2 positionOffset)
    {
        panels = panelRects;
        currentOffset = positionOffset;
        BaseOffset = positionOffset;
    }
    
    public static void AddOffServeListener(Action listener) => OffServeBtn += listener;
    public static void RemoveOffServeListener(Action lister) => OffServeBtn -= lister;
    public static void InvokeServe() => OffServeBtn?.Invoke();

    public bool GetArrowCentered() => isArrowCentered;

    public void SetRightArrow(RectTransform arrow)
    {
        rightArrow = arrow;
        rightArrowInitialPos = arrow.anchoredPosition;
    }

    public Vector2 GetRightArrowInitialPos() => rightArrowInitialPos;

    public void AnimateToOffset(Vector2 newOffset, float duration)
    {
        currentOffset = newOffset;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(AnimateMove(newOffset, duration));
    }

    private IEnumerator AnimateMove(Vector2 targetPos, float duration)
    {
        IsAnimating = true;

        Vector2[] startPositions = new Vector2[panels.Length];
        for (int i = 0; i < panels.Length; i++)
            startPositions[i] = panels[i].anchoredPosition;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            for (int i = 0; i < panels.Length; i++)
                panels[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPos, t);
            yield return null;
        }

        for (int i = 0; i < panels.Length; i++)
            panels[i].anchoredPosition = targetPos;

        IsAnimating = false;
    }

    public void MovePanelToCenter(Vector2 centerOffset, float duration)
    {
        if (!isArrowCentered)
        {
            AnimateToOffset(centerOffset, duration);

            if (rightArrow != null)
            {
                float offsetX = centerOffset.x - BaseOffset.x;
                Vector2 newArrowPos = rightArrow.anchoredPosition + new Vector2(offsetX, 0);
                StartCoroutine(MoveArrowToUIPosition(rightArrow, newArrowPos, duration));
            }

            isArrowCentered = true;
        }
        else
        {
            AnimateToOffset(BaseOffset, duration);

            if (rightArrow != null)
                StartCoroutine(MoveArrowToUIPosition(rightArrow, rightArrowInitialPos, duration));

            isArrowCentered = false;
        }
    }

    private IEnumerator MoveArrowToUIPosition(RectTransform arrow, Vector2 targetPos, float duration)
    {
        Vector2 startPos = arrow.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            arrow.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        arrow.anchoredPosition = targetPos;
    }
    public void ZoomAndFrameTargetLeftCenter(
        Camera cam,
        Transform target,                 // 클릭한 Customer transform
        float targetZoom,                 // orthographicSize or FOV
        float duration,                   // 카메라 팬+줌 시간
        Vector2 centerOffset,             // 인벤토리 중앙 위치(anchoredPosition)
        float moveDuration,               // 인벤토리 이동 시간
        CanvasGroup[] arrowGroups = null, // 화살표 CanvasGroups
        bool deactivateArrows = true,     // 화살표 비활성화 여부
        float viewportX = 0.2f,           // 화면 왼쪽 비율 (0=좌,1=우)
        float viewportY = 0.5f            // 화면 중앙 (0=하,1=상)
    )
    {
        if (focusRoutine != null) StopCoroutine(focusRoutine);
        originalCamPos = cam.transform.position;
        focusRoutine = StartCoroutine(ZoomAndFrameTargetLeftCenterRoutine(
            cam, target, targetZoom, duration, centerOffset, moveDuration, arrowGroups, deactivateArrows, viewportX, viewportY
        ));
    }

    private IEnumerator ZoomAndFrameTargetLeftCenterRoutine(
        Camera cam,
        Transform target,
        float targetZoom,
        float duration,
        Vector2 centerOffset,
        float moveDuration,
        CanvasGroup[] arrowGroups,
        bool deactivateArrows,
        float viewportX,
        float viewportY
    )
    {
        // 1) 화살표 페이드아웃 & 비활성화
        if (arrowGroups != null)
        {
            _lastOpaqueArrows = new HashSet<CanvasGroup>(
                arrowGroups.Where(cg => cg != null && cg.alpha >= 0.99f)
            );

            foreach (var cg in arrowGroups)
                if (cg != null) StartCoroutine(FadeCanvasGroup(cg, 0f, 0.25f));
            if (deactivateArrows)
                StartCoroutine(DeactivateAfterDelay(arrowGroups, 0.3f));
        }

        // 2) 인벤토리 중앙 이동
        MovePanelToCenter(centerOffset, moveDuration);

        // 3) 카메라 팬 + 줌 (타깃을 뷰포트 좌표 (viewportX, viewportY)에 오도록)
        if (cam != null && target != null)
            yield return StartCoroutine(PanAndZoomCameraToViewport(cam, target.position, targetZoom, duration, viewportX, viewportY));

        while (IsAnimating)
            yield return null;

    }

    private IEnumerator PanAndZoomCameraToViewport(
        Camera cam,
        Vector3 worldTarget,
        float targetZoom,
        float duration,
        float viewportX,
        float viewportY
    )
    {
        // 현재 카메라-타깃 거리 계산 (원근/직교 모두 호환)
        float distance;
        {
            var camSpace = cam.worldToCameraMatrix.MultiplyPoint3x4(worldTarget);
            distance = Mathf.Abs(camSpace.z);
            if (distance < 0.001f) distance = 0.001f; // 안전빵
        }

        // 타깃이 viewportX, viewportY에 오도록 필요한 "카메라 목표 위치" 계산
        Vector3 desiredWorldAtViewport = cam.ViewportToWorldPoint(new Vector3(viewportX, viewportY, distance));
        Vector3 camTargetPos = cam.transform.position + (worldTarget - desiredWorldAtViewport);

        // 시작 상태
        Vector3 startPos = cam.transform.position;
        float startZoom = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            cam.transform.position = Vector3.Lerp(startPos, camTargetPos, t);

            if (cam.orthographic)
                cam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
            else
                cam.fieldOfView = Mathf.Lerp(startZoom, targetZoom, t);

            yield return null;
        }

        cam.transform.position = camTargetPos;
        if (cam.orthographic) cam.orthographicSize = targetZoom;
        else cam.fieldOfView = targetZoom;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float target, float duration)
    {
        if (cg == null) yield break;
        float start = cg.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
        cg.alpha = target;
    }

    private IEnumerator DeactivateAfterDelay(CanvasGroup[] groups, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var cg in groups)
        {
            if (cg == null) continue;


            if (cg.alpha <= 0.001f)
            {
                cg.gameObject.SetActive(false);
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    public void ResetFromCenterWithCamera(
        Camera cam,
        float originalZoom,
        float zoomDuration,
        float moveDuration,
        CanvasGroup[] arrowGroups = null,
        bool activateArrows = true
    )
    {

        if (focusRoutine != null) StopCoroutine(focusRoutine);
        focusRoutine = StartCoroutine(ResetFromCenterWithCameraRoutine(
            cam, originalZoom, zoomDuration, moveDuration, arrowGroups, activateArrows
        ));
    }

    private IEnumerator ResetFromCenterWithCameraRoutine(
        Camera cam,
        float originalZoom,
        float zoomDuration,
        float moveDuration,
        CanvasGroup[] arrowGroups,
        bool activateArrows
    )
    {
        // 1) 패널 원위치 복귀
        MovePanelToCenter(BaseOffset, moveDuration);

        // 2) 카메라 위치/줌 복귀
        if (cam != null)
        {
            Vector3 startPos = cam.transform.position;
            float startZoom = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / zoomDuration;

                cam.transform.position = Vector3.Lerp(startPos, originalCamPos, t);
                if (cam.orthographic)
                    cam.orthographicSize = Mathf.Lerp(startZoom, originalZoom, t);
                else
                    cam.fieldOfView = Mathf.Lerp(startZoom, originalZoom, t);

                yield return null;
            }

            cam.transform.position = originalCamPos;
            if (cam.orthographic) cam.orthographicSize = originalZoom;
            else cam.fieldOfView = originalZoom;
        }

        // 3) 화살표 복귀
        if (arrowGroups != null && activateArrows)
        {
            foreach (var cg in arrowGroups)
            {
                if (cg != null)
                {
                    cg.gameObject.SetActive(true);
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                    //StartCoroutine(FadeCanvasGroup(cg, 1f, 0.25f));
                }
                
            }
            if (_lastOpaqueArrows != null)
            {
                foreach (var cg in _lastOpaqueArrows)
                {
                    if (cg == null) continue;;
                    StartCoroutine(FadeCanvasGroup(cg, 1f, 0.25f));
                }
            }
        }

        invmanager.ChangeToInventory();
    }

    public void SwapToFoodPanel(Transform ingredientPanel, Transform foodPanel, Image ingredientImg, Image foodImg)
    {
        var ingredientState = ingredientImg.GetComponent<PanelState>();
        var foodState = foodImg.GetComponent<PanelState>();

        if (ingredientState != null) ingredientImg.sprite = ingredientState.UnSelectedImg;
        if (foodState != null) foodImg.sprite = foodState.SelectedImg;

        if (foodPanel.GetSiblingIndex() < ingredientPanel.GetSiblingIndex())
        {
            foodPanel.SetSiblingIndex(ingredientPanel.GetSiblingIndex());
        }
    }

    public void SwapToIngredientPanel(Transform ingredientPanel, Transform foodPanel, Image ingredientImg, Image foodImg)
    {
        var ingredientState = ingredientImg.GetComponent<PanelState>();
        var foodState = foodImg.GetComponent<PanelState>();

        if (ingredientState != null) ingredientImg.sprite = ingredientState.SelectedImg;
        if (foodState != null) foodImg.sprite = foodState.UnSelectedImg;

        if (ingredientPanel.GetSiblingIndex() < foodPanel.GetSiblingIndex())
        {
            ingredientPanel.SetSiblingIndex(foodPanel.GetSiblingIndex());
        }
    }
}
