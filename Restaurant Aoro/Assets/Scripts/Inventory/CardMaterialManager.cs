using UnityEngine;
using UnityEngine.UI;

public class CardMaterialManager : MonoBehaviour
{
    public static CardMaterialManager Instance;

    public GameObject parentCard; // 카드 부모 오브젝트
    public GameObject parentItemSlot; // 아이템 슬롯 부모 오브젝트

    private UICard[] allCards;
    private UIItemSlot[] allItemSlots;

    private UICard selectedCard;

    private void Awake()
    {
        Instance = this;

        if (parentCard == null || parentItemSlot == null)
        {
            Debug.LogError("부모 오브젝트가 설정되지 않았습니다.");
            return;
        }

        allCards = parentCard.GetComponentsInChildren<UICard>(includeInactive: true);
        allItemSlots = parentItemSlot.GetComponentsInChildren<UIItemSlot>(includeInactive: true);
    }

    public void SelectCard(UICard card)
    {
        selectedCard = card;

        UIOverlayMask.Instance.Show();

        foreach (UICard c in allCards)
            c.GetComponent<Button>().interactable = false;

        card.GetComponent<Button>().interactable = true;

        foreach (UIItemSlot slot in allItemSlots)
            slot.SetInteractable(true);
    }

    public void AssignToSelectedCard(ItemData item, UIItemSlot slot)
    {
        if (selectedCard == null) return;

        selectedCard.AssignMaterial(item);
        slot.SetInteractable(false);
        CloseOverlay();
    }

    public void UnassignFromCard(UICard card, ItemData item)
    {
        // 아이템 복구
        foreach (var slot in allItemSlots)
        {
            if (slot.item == item)
                slot.SetInteractable(true);
        }
    }

    private void CloseOverlay()
    {
        selectedCard = null;

        foreach (var c in allCards)
            c.GetComponent<Button>().interactable = true;

        foreach (var slot in allItemSlots)
            slot.SetInteractable(false); // 기본적으로 비활성화

        UIOverlayMask.Instance.Hide();
    }
}
