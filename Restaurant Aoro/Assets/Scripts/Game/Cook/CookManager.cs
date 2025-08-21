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

        private void OnEnable()
        {
            if (instance != null) return;

            instance = this;
        }

        public void PrepareBackground(GameObject obj)
        {
            var slide = background.GetComponent<SlidingController>();
            slide.targetTransforms[1] = obj.transform;
            slide.SlideIn();

            foreach (var btn in cookMenuBtns)
            {
                var b = btn.gameObject;
                if (b.name != obj.name)
                {
                    b.GetComponent<FadingController>().FadeOut(false);
                }
            }

            background.GetComponent<ResizingController>().ResizeIn();
        }

        public void EnterCook()
        {
            if (inventoryManager.isCentered) return;

            var slide = cooks.GetComponent<ISlideIn>();
            if (slide == null) throw new Exception("No cook objects!");

            cooks.SetActive(true);
            slide.SlideIn(true, () =>
            {
                foreach (var cookType in cookTypeBtns)
                {
                    cookType.FadeIn();
                }
            });

            inventoryManager.OnClickToggleInventoryPosition();
            arrowController.MoveArrowsOutOfScreen();
            backBtn.SlideIn();
        }

        public void ExitCook()
        {
            var slide = cooks.GetComponent<ISlideOut>();
            if (slide == null) throw new Exception("No cook objects!");

            slide.SlideOut(false, () =>
            {
                foreach (var cookType in cookTypeBtns)
                {
                    cookType.FadeOut(false);
                }

                cooks.SetActive(false);

                if (inventoryManager.isCentered)
                    inventoryManager.OnClickToggleInventoryPosition();

                background.GetComponent<SlidingController>().SlideOut(true);
                background.GetComponent<ResizingController>().ResizeOut(true, () =>
                {
                    foreach (var btn in cookMenuBtns)
                    {
                        btn.gameObject.SetActive(true);
                        btn.SlideOut(true, () =>
                        {
                            if (btn.GetComponent<SpriteRenderer>().color.a != 1)
                            {
                                btn.GetComponent<FadingController>().FadeIn();
                            }
                        });
                    }
                });

                arrowController.MoveArrowsInToScreen();
                backBtn.SlideOut(true);
            });
        }
    }
}