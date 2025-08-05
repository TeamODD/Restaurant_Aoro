using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryController : MonoBehaviour
{
    private RectTransform[] panels;
    private Vector2 currentOffset;
    private RectTransform rightArrow { get; set; }
    private Vector2 rightArrowInitialPos;
    private bool isArrowCentered = false;

    private Coroutine moveCoroutine;

    public Vector3 BaseOffset { get; private set; }
    public Vector3 CurrentOffset => currentOffset;
    public bool IsAnimating { get; private set; }

    public void SetIsAnimating(bool value) => IsAnimating = value;

    public void Initialize(RectTransform[] panelRects, Vector2 positionOffset)
    {
        panels = panelRects;
        currentOffset = positionOffset;
        BaseOffset = positionOffset;
    }

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
