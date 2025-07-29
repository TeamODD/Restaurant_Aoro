using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class IngredientCard : MonoBehaviour
    {
        public Image iconImage;
        public Button cardButton;
        public Button clearButton;

        public ItemData CurrentItem { get; private set; }

        // 자신의 원래 위치를 기억하기 위한 변수
        private Transform originalParent;
        private int originalSiblingIndex;

        private void Start()
        {
            // 시작할 때 자신의 원래 부모와 순서를 저장
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();

            cardButton.onClick.AddListener(OnCardClick);
            clearButton.onClick.AddListener(OnClearClick);
            UpdateCardView();
        }

        private void OnCardClick()
        {
            CardInteractionManager.Instance.OnCardSelected(this);
        }

        private void OnClearClick()
        {
            CardInteractionManager.Instance.OnCardCleared(this);
        }

        public void AssignItem(ItemData item)
        {
            CurrentItem = item;
            UpdateCardView();
        }

        public void ClearItem()
        {
            CurrentItem = null;
            UpdateCardView();
        }

        private void UpdateCardView()
        {
            if (CurrentItem != null)
            {
                iconImage.sprite = CurrentItem.icon;
                iconImage.enabled = true;
                clearButton.gameObject.SetActive(true);
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                clearButton.gameObject.SetActive(false);
            }
        }

        // 카드를 화면 맨 앞으로 가져오는 메서드
        public void BringToFront(Transform topLevelParent)
        {
            transform.SetParent(topLevelParent, true);
            transform.SetAsLastSibling(); // 맨 마지막에 렌더링되도록(가장 위에 보이도록) 함
        }

        // 카드를 원래 위치로 되돌리는 메서드
        public void ReturnToOriginalPosition()
        {
            transform.SetParent(originalParent, true);
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }
}
