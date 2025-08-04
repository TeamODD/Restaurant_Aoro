using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryController : MonoBehaviour
{
    private Camera mainCamera;
    private Transform[] panels;
    private Vector3 currentOffset;
    private Transform rightArrow { get; set; }
    private Vector3 rightArrowInitialPos;
    private bool isArrowCentered = false;

    private Vector3 currentTargetPos;
    private Quaternion currentTargetRot;

    private Coroutine moveCoroutine;

    public Vector3 BaseOffset { get; private set; }
    public Vector3 CurrentOffset => currentOffset;
    public bool IsAnimating { get; private set; }

    public void SetIsAnimating(bool value) => IsAnimating = value;

    public void Initialize(Camera cam, Transform[] panelTransforms, Vector3 positionOffset)
    {
        mainCamera = cam;
        panels = panelTransforms;
        currentOffset = positionOffset;
        BaseOffset = positionOffset;
    }

    public bool GetArrowCentered()
    {
        return isArrowCentered;
    }

    public void SetrightArrowInitialPos(Vector3 pos)
    {
        rightArrowInitialPos = pos;
    }
    public Vector3 GetrightArrowInitialPos()
    {
        return rightArrowInitialPos;
    }

    public void AnimateToOffset(Vector3 newOffset, Vector3 cameraFinalPos, float duration, float rotateSpeed = 10f)
    {
        currentOffset = newOffset;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(AnimateMove(cameraFinalPos, duration, rotateSpeed));
    }

    private IEnumerator AnimateMove(Vector3 cameraFinalPos, float duration, float rotateSpeed)
    {
        IsAnimating = true;

        // 목표 위치 계산 (이동 중에는 매 프레임마다 새로 계산 X, 카메라 위치 고정 기준)
        Vector3 camRight = mainCamera.transform.right;
        Vector3 camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;

        Vector3 fixedTargetPos = cameraFinalPos + camRight * currentOffset.x + Vector3.up * currentOffset.y + camForward * currentOffset.z;
        Quaternion fixedTargetRot = Quaternion.LookRotation(camForward);

        Vector3[] startPositions = new Vector3[panels.Length];
        Quaternion[] startRotations = new Quaternion[panels.Length];

        for (int i = 0; i < panels.Length; i++)
        {
            startPositions[i] = panels[i].position;
            startRotations[i] = panels[i].rotation;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].position = Vector3.Lerp(startPositions[i], fixedTargetPos, t);
                panels[i].rotation = Quaternion.Slerp(startRotations[i], fixedTargetRot, t * rotateSpeed);
            }

            yield return null;
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].position = fixedTargetPos;
            panels[i].rotation = fixedTargetRot;
        }

        IsAnimating = false;
    }

    public void UpdatePanelPositions()
    {
        if (mainCamera == null || panels == null || panels.Length == 0) return;

        UpdateTargetTransform(); //항상 카메라 기준 위치를 계산

        // 애니메이션 중에는 보간 유지
        if (IsAnimating)
        {
            // 아무것도 하지 않음 (애니메이션이 위치를 바꿈)
            return;
        }

        // 애니메이션이 아닐 경우에만 강제로 위치 고정
        foreach (Transform panel in panels)
        {
            panel.position = currentTargetPos;
            panel.rotation = currentTargetRot;
        }
    }

    private void UpdateTargetTransform()
    {
        Vector3 camRight = mainCamera.transform.right;
        Vector3 camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;

        currentTargetPos = mainCamera.transform.position + camRight * currentOffset.x + Vector3.up * currentOffset.y + camForward * currentOffset.z;
        currentTargetRot = Quaternion.LookRotation(camForward);
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

    public void MovePanelToCenter(Vector3 centerOffset, float duration)
    {
        Vector3 cameraFinalPos = mainCamera.transform.position;

        if (!isArrowCentered)
        {
            AnimateToOffset(centerOffset, cameraFinalPos, duration);

            if (rightArrow != null)
            {
                // 처음 위치 기억
                rightArrowInitialPos = rightArrow.position;

                // X축 이동량 계산
                float offsetX = centerOffset.x - BaseOffset.x;
                Vector3 newArrowPos = rightArrow.position + mainCamera.transform.right * offsetX;

                StartCoroutine(MoveArrowToPosition(rightArrow, newArrowPos, duration));
            }

            isArrowCentered = true;
        }
        else
        {
            AnimateToOffset(BaseOffset, cameraFinalPos, duration);

            if (rightArrow != null)
            {
                StartCoroutine(MoveArrowToPosition(rightArrow, rightArrowInitialPos, duration));
            }

            isArrowCentered = false;
        }
    }

    private IEnumerator MoveArrowToPosition(Transform arrow, Vector3 targetPos, float duration)
    {
        Vector3 startPos = arrow.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            arrow.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        arrow.position = targetPos;
    }

    public void SetRightArrow(Transform arrow)
    {
        rightArrow = arrow;
    }
}
