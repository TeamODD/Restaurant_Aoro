using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameUIController : MonoBehaviour
{
    [Header("Save Slots")]
    [SerializeField] private Transform slotParent;

    [Header("Back Button")]
    [SerializeField] private Obj_Production backButton;

    [Header("Transition")]
    [SerializeField] private Image blackOverlay;

    [Header("Animation Settings")]
    [SerializeField] private float moveTime = 0.35f;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float slotMoveDistance = 1000f;
    [SerializeField] private float backMoveDistance = 1000f;

    private bool isAnimating = false;
    private void Start()
    {
        Color color = blackOverlay.color;
        color.a = 0f;

        blackOverlay.color = color;

        blackOverlay.gameObject.SetActive(false);
    }

    public void SelectSlot(GameObject selectedSlot)
    {
        if (isAnimating)
            return;

        StartCoroutine(
            LoadGameSequence(selectedSlot)
        );
    }

    private IEnumerator LoadGameSequence(
        GameObject selectedSlot)
    {
        isAnimating = true;

        foreach (Transform child in slotParent)
        {
            Obj_Production production =
                child.GetComponent<Obj_Production>();

            if (production == null)
                continue;

            float currentY =
                child.localPosition.y;

            if (child.gameObject == selectedSlot)
            {
                production.Move(
                    "Smooth",
                    true,
                    moveTime,
                    "y",
                    currentY,
                    currentY + slotMoveDistance
                );
            }
            else
            {
                production.Move(
                    "Smooth",
                    true,
                    moveTime,
                    "y",
                    currentY,
                    currentY - slotMoveDistance
                );
            }
        }

        if (backButton != null)
        {
            float currentX =
                backButton.transform.localPosition.x;

            backButton.Move(
                "Smooth",
                true,
                moveTime,
                "x",
                currentX,
                currentX - backMoveDistance
            );
        }

        yield return new WaitForSeconds(moveTime);

        blackOverlay.gameObject.SetActive(true);

        Color startColor = blackOverlay.color;
        startColor.a = 0f;

        Color endColor = startColor;
        endColor.a = 1f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            float t =
                elapsedTime / fadeTime;

            blackOverlay.color =
                Color.Lerp(
                    startColor,
                    endColor,
                    t
                );

            yield return null;
        }

        blackOverlay.color = endColor;

        SceneManager.LoadScene("Map");
    }
}