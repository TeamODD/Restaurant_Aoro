using UnityEngine;
using UnityEngine.UI;
using System;

namespace Inventory
{
    public class CardInteractionManager : MonoBehaviour
    {
        public static CardInteractionManager Instance { get; private set; }

        public Image darkOverlay;
        public GameObject mockInventoryPanel;

        // 최상위 Canvas의 Transform을 연결해줍니다.
        public Transform mainCanvasTransform;

        private IngredientCard selectedCard;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (darkOverlay != null)
            {
                darkOverlay.gameObject.SetActive(false);
            }
            if (mockInventoryPanel != null)
            {
                mockInventoryPanel.SetActive(false);
            }
        }

        public void OnCardSelected(IngredientCard card)
        {
            if (card.CurrentItem == null)
            {
                selectedCard = card;

                // 1. 오버레이를 카드와 인벤토리 바로 뒤에 위치시킵니다.
                darkOverlay.transform.SetAsLastSibling();
                if (mockInventoryPanel != null) mockInventoryPanel.transform.SetAsLastSibling();
                selectedCard.transform.SetAsLastSibling();

                darkOverlay.gameObject.SetActive(true);

                // 2. 선택된 카드를 맨 앞으로 가져옵니다.
                selectedCard.BringToFront(mainCanvasTransform);

                if (mockInventoryPanel != null)
                {
                    // 3. 인벤토리 패널도 맨 앞으로 가져옵니다.
                    mockInventoryPanel.transform.SetAsLastSibling();
                    mockInventoryPanel.SetActive(true);
                }
            }
        }

        public void OnIngredientSelectedFromInventory(ItemData ingredient)
        {
            if (selectedCard != null)
            {
                // 4. 카드를 원래 위치로 되돌립니다.
                selectedCard.ReturnToOriginalPosition();

                selectedCard.AssignItem(ingredient);
                darkOverlay.gameObject.SetActive(false);

                if (mockInventoryPanel != null)
                {
                    mockInventoryPanel.SetActive(false);
                }

                selectedCard = null;
            }
        }

        public void OnCardCleared(IngredientCard card)
        {
            ItemData itemToReturn = card.CurrentItem;
            card.ClearItem();

            Debug.Log($"{itemToReturn.name} 아이템이 (가상)인벤토리로 반환되었습니다.");
        }
    }
}

