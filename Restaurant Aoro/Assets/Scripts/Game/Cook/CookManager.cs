using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.UI;
using Unity.VisualScripting;
using UnityEngine;

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
        [HideInInspector] public CookType cookType = CookType.None;
        private GameObject itemOnHold;
        private CookMenuBtn menuBtnOnHold;
        [SerializeField] private CookTile[] cookTiles;
        [SerializeField] private SlidingController cookBtn;
        [SerializeField] private GameObject tileOverlay;
        private bool isWorking;
        public float cookingTime = 60f;

        private void OnEnable()
        {
            if (instance != null) return;

            instance = this;
        }

        public bool PrepareBackground(CookMenuBtn obj)
        {
            if (inventoryManager.isCentered || isWorking) return false;

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

            return true;
        }

        public void EnterCook()
        {
            var slide = cooks.GetComponent<ISlideIn>();

            cooks.SetActive(true);
            slide.SlideIn(true, () =>
            {
                foreach (var cookTypeBtn in cookTypeBtns)
                {
                    cookTypeBtn.gameObject.SetActive(true);
                    cookTypeBtn.FadeIn(true, () => isWorking = false);
                    cookBtn.gameObject.SetActive(true);
                    cookBtn.SlideIn(true);
                }
                
                inventoryManager.EnableDrag();
            });

            inventoryManager.OnClickToggleInventoryPosition();
            arrowController.MoveArrowsOutOfScreen();
            backBtn.SlideIn(true);
        }

        public void AddIngredientToCookTile(GameObject obj, bool activeOverlay = true)  
        {
            if (obj.GetComponent<ItemSlotUI>().item_.ItemType != ItemType.Ingredient) return;

            tileOverlay.SetActive(activeOverlay);
            itemOnHold = obj;
        }

        public void IngredientAddedToCookTile(CookTile cookTile)
        {
            if (!itemOnHold) return;

            var destroy = cookTile.AddItem(itemOnHold.GetComponent<ItemSlotUI>().item_);
            if (destroy) Destroy(itemOnHold.transform.parent.gameObject);
            itemOnHold = null;
            tileOverlay.SetActive(false);
        }

        public void Cook()
        {
            if (cookType == CookType.None) return;
         
            Debug.Log("[CookManager] Cook Started!");
            var ingredients = new List<Item>();

            foreach (var cookTile in cookTiles)
            {
                if (cookTile.item == null)
                {
                    Debug.Log("Cook Tile is empty!");
                    return;
                }

                ingredients.Add(cookTile.item);
            }

            var cookFactory = new CookFactory();
            var ingredientsByMainCategory = ingredients.OrderBy(item => item.ItemMainCategory).ToArray();
            var ingredientsBySubCategory = ingredients.OrderBy(item => item.ItemSubCategory).ToArray();
            
            for (var i = 0; i < ingredients.Count; i++)
            {
                var mainCategory = ingredientsByMainCategory[i].ItemMainCategory;
                for (var j = 0; j < ingredients.Count; j++)
                {
                    var subCategory = ingredientsBySubCategory[j].ItemSubCategory;
                    var result = cookFactory.Make(mainCategory, subCategory);
                    if (!result) continue;
                    Debug.Log($"[CookManager] Will output {result.ItemName}");
                    ExitCook(result);
                    return;
                }
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
            ExitCook(null);
        }

        public void ExitCook(Item cookItem)
        {
            if (isWorking) return;

            inventoryManager.DisableDrag();
            
            isWorking = true;

            cookBtn.SlideOut(false, () =>
            {
                cooks.GetComponent<ISlideOut>().SlideOut(false, () =>
                {
                    cooks.SetActive(false);

                    if (inventoryManager.isCentered)
                        inventoryManager.OnClickToggleInventoryPosition();

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
                    }
                    
                    foreach (var btn in cookMenuBtns)
                    {
                        btn.gameObject.SetActive(true);
                        btn.GetComponent<BoxCollider2D>().enabled = true;
                        btn.SlideOut(true, () =>
                        {
                            if (!Mathf.Approximately(btn.GetComponent<SpriteRenderer>().color.a, 1))
                            {
                                btn.GetComponent<FadingController>().FadeIn();
                            }
                        });
                    }

                    background.FadeIn();

                    arrowController.MoveArrowsInToScreen();
                    backBtn.SlideOut(true, () => isWorking = false);
                });

                foreach (var cookTypeBtn in cookTypeBtns)
                {
                    cookTypeBtn.FadeOut(false);
                }
            });
        }
    }
}