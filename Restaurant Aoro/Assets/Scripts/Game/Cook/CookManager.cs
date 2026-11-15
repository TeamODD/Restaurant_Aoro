using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Cook
{
    public enum CookType
    {
        None,
        UseHeat,
        NonHeat,
    }

    public class CookManager : MonoBehaviour
    {
        public static CookManager instance;

        [SerializeField] private GameObject cooks;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private ArrowController arrowController;
        [SerializeField] private SlidingController backBtn;
        [SerializeField] private SlidingController[] cookMenuBtns;
        [SerializeField] private FadingController[] cookTypeBtns;
        [SerializeField] private FadingController background;
        [HideInInspector] public CookType cookType = CookType.UseHeat;
        private CookMenuBtn menuBtnOnHold;
        private CookTile cooktileOnHold;
        [SerializeField] private CookTile[] cookTiles;
        [SerializeField] private SlidingController cookBtn;
        [SerializeField] private GameObject tileOverlay;
        private bool isWorking;
        public float cookingTime = 60f;
        [SerializeField] private GradePolicy gradePolicy;

        private RecipeCodexEntry selectedRecipe;

        public bool IsRecipeMode => selectedRecipe != null;

        private void OnEnable()
        {
            if (instance != null) return;

            instance = this;
        }

        public bool PrepareBackground(CookMenuBtn obj)
        {
            if (isWorking) return false;

            isWorking = true;
            menuBtnOnHold = obj;

            foreach (var btn in cookMenuBtns)
            {
                var b = btn.gameObject;
                if (b.name != obj.name)
                {
                    b.GetComponent<FadingController>().FadeOut(false);
                }
                else
                {
                    b.GetComponent<FadingController>().FadeIn(true, () => { background.FadeOut(); });
                    b.GetComponent<BoxCollider2D>().enabled = false;
                }
            }

            arrowController.MoveArrowsOutOfScreen();
            if (!inventoryManager.isCentered)
            {
                inventoryManager.OnClickToggleInventoryPosition();
                inventoryManager.ChangeToIngredientInventory();
            }

            return true;
        }

        public void EnterCook()
        {
            var slide = cooks.GetComponent<ISlideIn>();

            cooks.SetActive(true);
            slide.SlideIn(true, () =>
            {
                cookBtn.gameObject.SetActive(true);
                cookBtn.SlideIn(true, () =>
                {
                    // foreach (var cookTypeBtn in cookTypeBtns)
                    // {
                    //     cookTypeBtn.gameObject.SetActive(true);
                    //     cookTypeBtn.FadeIn(true, () => );
                    // }
                });
                isWorking = false;
                inventoryManager.EnableDrag();
            });

            //arrowController.MoveArrowsOutOfScreen();
            backBtn.SlideIn(true);
        }

        public void AddIngredientToCookTile(CookTile obj, bool activeOverlay = true)
        {
            tileOverlay.SetActive(activeOverlay);
            cooktileOnHold = obj;
        }

        public void ResetTileSortOrder()
        {
            foreach (var tile in cookTiles)
            {
                tile.GetComponent<SpriteRenderer>().sortingOrder = 5;
            }
        }

        public void IngredientAddedToCookTile(ItemSlotUI item)
        {
            if (!cooktileOnHold || item.item_.ItemType != ItemType.Ingredient) return;

            var destroy = cooktileOnHold.AddItem(item.item_);
            if (destroy) Destroy(item.GameObject().transform.parent.gameObject);
            cooktileOnHold = null;
            tileOverlay.SetActive(false);
            ResetTileSortOrder();
        }

        public void ResetCookTileOnHold()
        {
            cooktileOnHold = null;
        }

        public void Cook()
        {
            // if (cookType == CookType.None) return;

            Debug.Log("[CookManager] Cook Started!");
            var ingredients = (from cookTile in cookTiles where cookTile.item select cookTile.item).ToList();

            var cookFactory = new CookFactory(gradePolicy);

            var result = cookFactory.Make(ingredients.ToArray());
            if (result)
            {
                Debug.Log($"[CookManager] Will output {result.ItemName}");
                //add codex
                RecipeCodexManager.Instance.RegisterRecipe(ingredients, result);
                ExitCook(result);
                return;
            }

            Debug.Log("[CookManager] No available recipe for this set!");
        }

        private IEnumerator CookingCoroutine(Item cookItem)
        {
            yield return new WaitForSeconds(cookingTime);
            inventoryManager.AddItem(cookItem);
            Debug.Log("[CookManager] Cooking Complete!");
            //Add notifier here!
        }

        public void ExitCookBtn()
        {
            if (isWorking) return;
            ExitCook(null);
        }

        private void ExitCook(Item cookItem)
        {
            if (isWorking) return;

            inventoryManager.DisableDrag();

            isWorking = true;

            Debug.Log("A");
            cookBtn.SlideOut(false, () =>
            {
                Debug.Log("B");
                cooks.GetComponent<ISlideOut>().SlideOut(false, () =>
                {
                    Debug.Log("C");
                    cooks.SetActive(false);

                    if (inventoryManager.isCentered)
                        inventoryManager.OnClickToggleInventoryPosition();
                    inventoryManager.ChangeToInventory();

                    if (cookItem)
                    {
                        Debug.Log("[CookManager] Cleaning Cook Elements");

                        menuBtnOnHold.StartCookingBar(cookingTime);
                        menuBtnOnHold = null;

                        StartCoroutine(CookingCoroutine(cookItem));

                        foreach (var cookTile in cookTiles)
                        {
                            cookTile.RemoveItemWithoutAdding();
                        }
                    }
                    else
                    {
                        foreach (var cookTile in cookTiles)
                        {
                            cookTile.RemoveItem();
                        }

                        menuBtnOnHold = null;
                    }

                    foreach (var btn in cookMenuBtns)
                    {
                        btn.gameObject.SetActive(true);
                        btn.GetComponent<BoxCollider2D>().enabled = true;
                        btn.SlideOut(true, () =>
                        {
                            Debug.Log("D");
                            if (!Mathf.Approximately(btn.GetComponent<SpriteRenderer>().color.a, 1))
                            {
                                btn.GetComponent<FadingController>().FadeIn();
                            }
                        });
                    }

                    background.FadeIn();

                    arrowController.MoveArrowsInToScreen();
                    backBtn.SlideOut(true, () =>
                    {
                        Debug.Log("E");
                        isWorking = false;
                    });
                });

                // cookType = CookType.None;
                // foreach (var cookTypeBtn in cookTypeBtns)
                // {
                //     cookTypeBtn.FadeOut(false);
                // }
            });
        }
        public void BeginRecipeCook(RecipeCodexEntry recipe)
        {
            if (recipe == null || recipe.result == null)
                return;

            selectedRecipe = recipe;

            Debug.Log($"[CookManager] 레시피 요리 모드 시작: {recipe.result.itemId}");
        }
        public void SelectRecipeCookSlot(CookMenuBtn slot)
        {
            if (!IsRecipeMode || slot == null || isWorking)
                return;

            if (!slot.IsAvailable)
            {
                Debug.Log("[CookManager] 사용 중인 조리 슬롯입니다.");
                return;
            }

            if (selectedRecipe.ingredients == null)
                return;

            Item resultItem = ItemDatabase.Instance.GetItem(
                selectedRecipe.result.itemId
            );

            if (resultItem == null)
            {
                Debug.LogWarning("[CookManager] 레시피 결과 아이템을 찾을 수 없습니다.");
                return;
            }

            // 레시피에 필요한 재료 수량 계산
            Dictionary<string, int> requiredItems = new();

            foreach (var ingredient in selectedRecipe.ingredients)
            {
                if (string.IsNullOrEmpty(ingredient.itemId))
                    return;

                if (requiredItems.ContainsKey(ingredient.itemId))
                    requiredItems[ingredient.itemId]++;
                else
                    requiredItems[ingredient.itemId] = 1;
            }

            // 모든 재료의 보유량 확인
            foreach (var required in requiredItems)
            {
                if (inventoryManager.GetItemCount(required.Key) < required.Value)
                {
                    Debug.LogWarning(
                        $"[CookManager] 재료 부족: {required.Key}, 필요 {required.Value}개"
                    );
                    return;
                }
            }

            // 재료 소모
            foreach (var required in requiredItems)
            {
                Item ingredientItem = ItemDatabase.Instance.GetItem(required.Key);

                if (ingredientItem == null)
                {
                    Debug.LogWarning($"[CookManager] 재료 아이템을 찾을 수 없습니다: {required.Key}");
                    return;
                }

                for (int i = 0; i < required.Value; i++)
                {
                    inventoryManager.ConsumeItem(ingredientItem);
                }
            }

            // 조리 시작
            slot.StartCookingBar(cookingTime);
            StartCoroutine(CookingCoroutine(resultItem));

            Debug.Log($"[CookManager] 레시피 요리 시작: {resultItem.ItemName}");

            // 레시피 요리 모드 종료
            selectedRecipe = null;
            menuBtnOnHold = null;
        }
        public void CancelRecipeCook()
        {
            selectedRecipe = null;
            menuBtnOnHold = null;

            Debug.Log("[CookManager] 레시피 요리 모드 종료");
        }
    }


}