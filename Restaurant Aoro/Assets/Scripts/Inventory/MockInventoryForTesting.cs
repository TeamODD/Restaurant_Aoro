using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    // 테스트 전용 가짜 인벤토리 스크립트
    public class MockInventoryForTesting : MonoBehaviour
    {
        // Unity 에디터에서 테스트용 ItemData 3개를 연결해줍니다.
        public ItemData testIngredient1;
        public ItemData testIngredient2;
        public ItemData testIngredient3;

        // 각 재료에 해당하는 버튼을 에디터에서 연결해줍니다.
        public Button ingredientButton1;
        public Button ingredientButton2;
        public Button ingredientButton3;

        private void Start()
        {
            // 각 버튼이 클릭되면 어떤 재료를 선택할지 미리 지정해줍니다.
            ingredientButton1.onClick.AddListener(() => SelectIngredient(testIngredient1));
            ingredientButton2.onClick.AddListener(() => SelectIngredient(testIngredient2));
            ingredientButton3.onClick.AddListener(() => SelectIngredient(testIngredient3));
        }

        private void SelectIngredient(ItemData selectedIngredient)
        {
            // CardInteractionManager에 선택된 재료를 알려줍니다.
            CardInteractionManager.Instance.OnIngredientSelectedFromInventory(selectedIngredient);
        }
    }
}
