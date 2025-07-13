using DG.Tweening;
using Inventory;
using UnityEngine;

public class CardChainAnimator : MonoBehaviour
{
    public UICardMover[] cards; // 카드 0~4
    public RectTransform[] targetPositions; // 위치 0~4
    public RectTransform centerStackPosition; // 접힐 위치

    public float moveDuration = 0.3f;
    public float stepDelay = 0.05f;

    private bool isOpen = false;
    private bool isAnimating = false;


    private void Awake()
    {
        if (cards.Length != targetPositions.Length)
        {
            Debug.LogError("카드와 타겟 위치의 배열 길이가 같아야 합니다.");
            return;
        }

        CardActivation(false);
    }

    private void CardActivation(bool isActive)
    {
        foreach (UICardMover card in cards)
        {
            card.gameObject.SetActive(isActive);
        }
    }

    public void ToggleCardChain()
    {
        if (isAnimating) return;

        if (isOpen)
            CloseCardChainSequence();
        else
            StartCardChainSequence();

        isOpen = !isOpen;
    }

    public void StartCardChainSequence()
    {
        CardActivation(true);

        isAnimating = true;

        Vector2 startPos = targetPositions[0].anchoredPosition;

        foreach (var card in cards)
        {
            card.Initialize();
            card.MoveTo(startPos, moveDuration);
        }

        DOVirtual.DelayedCall(moveDuration + stepDelay, () =>
        {
            MoveStep(1);
        });
    }

    private void MoveStep(int step)
    {
        if (step >= cards.Length)
        {
            isAnimating = false;
            return;
        }

        Vector2 toPos = targetPositions[step].anchoredPosition;

        for (int i = step; i < cards.Length; i++)
        {
            cards[i].MoveTo(toPos, moveDuration);
        }

        DOVirtual.DelayedCall(moveDuration + stepDelay, () =>
        {
            MoveStep(step + 1);
        });
    }

    public void CloseCardChainSequence()
    {
        isAnimating = true;
        CloseStep(cards.Length - 1);
    }

    private void CloseStep(int step)
    {
        if (step <= 0)
        {
            Vector2 center = centerStackPosition.anchoredPosition;

            foreach (var card in cards)
            {
                card.MoveTo(center, moveDuration);
            }

            DOVirtual.DelayedCall(moveDuration, () =>
            {
                isAnimating = false;
                CardActivation(false);
            });

            return;
        }

        Vector2 toPos = targetPositions[step - 1].anchoredPosition;

        for (int i = step; i < cards.Length; i++)
        {
            cards[i].MoveTo(toPos, moveDuration);
        }

        DOVirtual.DelayedCall(moveDuration + stepDelay, () =>
        {
            CloseStep(step - 1);
        });
    }
}
