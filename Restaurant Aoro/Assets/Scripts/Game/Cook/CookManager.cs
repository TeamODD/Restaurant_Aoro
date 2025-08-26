using System;
using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookManager : MonoBehaviour
    {
        public static CookManager instance;

        [SerializeField] private GameObject cooks;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private ArrowController arrowController;
        [SerializeField] private SlidingController backBtn;
        [SerializeField] private SlidingController[] cookMenuBtns;
        [SerializeField] private FadingController[] cookTypeBtns;
        [SerializeField] private GameObject background;
        [HideInInspector] public bool useHeat;
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
            
            var slide = background.GetComponent<SlidingController>();

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
                        slide.targetTransforms[1] = obj.backgroundPosition;
                        slide.SlideIn();
                        
                        background.GetComponent<ResizingController>().ResizeIn();
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
                
                background.GetComponent<SlidingController>().SlideOut(true);
                background.GetComponent<ResizingController>().ResizeOut();
                
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