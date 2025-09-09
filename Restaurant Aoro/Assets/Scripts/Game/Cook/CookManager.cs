using System;
using System.Collections;
using Game.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Cook
{
    public enum CookType
    {
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
        [HideInInspector] public CookType cookType;
        private GameObject itemOnHold;
        [SerializeField] private GameObject tileOverlay;
        private bool isWorking;

        private void OnEnable()
        {
            if (instance != null) return;

            instance = this;
        }

        public bool PrepareBackground(CookMenuBtn obj)
        {
            if (inventoryManager.isCentered || isWorking) return false;

            isWorking = true;

            foreach (var btn in cookMenuBtns)
            {
                var b = btn.gameObject;
                if (b.name != obj.name)
                {
                    b.GetComponent<FadingController>().FadeOut(false);
                }
                else
                {
                    b.GetComponent<FadingController>().FadeIn(true, () =>
                    {
                        background.FadeOut();
                    });
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
                foreach (var cookType in cookTypeBtns)
                {
                    cookType.gameObject.SetActive(true);
                    cookType.FadeIn(true, () => isWorking = false);
                }
            });

            inventoryManager.OnClickToggleInventoryPosition();
            arrowController.MoveArrowsOutOfScreen();
            backBtn.SlideIn(true);
        }

        public void AddIngredientToCookTile(GameObject obj)
        {
            tileOverlay.SetActive(true);
            itemOnHold = obj;
        }

        public void IngredientAddedToCookTile(CookTile cookTile)
        {
            if (!itemOnHold) return;
            
            cookTile.AddItem(itemOnHold.GetComponent<ItemSlotUI>().item_);
            Destroy(itemOnHold.transform.parent.gameObject);
            itemOnHold = null;
            tileOverlay.SetActive(false);
        }

        public void Cook()
        {
            
        }

        public void ExitCook()
        {
            if (isWorking) return;

            isWorking = true;

            cooks.GetComponent<ISlideOut>().SlideOut(false, () =>
            {
                cooks.SetActive(false);

                if (inventoryManager.isCentered)
                    inventoryManager.OnClickToggleInventoryPosition();

                foreach (var btn in cookMenuBtns)
                {
                    btn.gameObject.SetActive(true);
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

            foreach (var cookType in cookTypeBtns)
            {
                cookType.FadeOut(false);
            }
        }
    }
}