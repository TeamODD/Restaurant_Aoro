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

        [HideInInspector] public bool useHeat;

        private void OnEnable()
        {
            if (instance != null) return;

            instance = this;
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
                cooks.SetActive(false);

                if (inventoryManager.isCentered)
                    inventoryManager.OnClickToggleInventoryPosition();

                foreach (var s in cookMenuBtns)
                {
                    s.gameObject.SetActive(true);
                    s.SlideOut(true);
                }

                arrowController.MoveArrowsInToScreen();
                backBtn.SlideOut(true);
            });
        }
    }
}