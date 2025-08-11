using System;
using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookMenuBtn : SlidingController
    {
        [SerializeField] private GameObject cooks;
        [SerializeField] private DisableOtherObjects disable;
        [SerializeField] private InventoryManager inventoryManager;
        private ISlideIn slide;

        private void OnMouseDown()
        {
            if (inventoryManager.isCentered) return;
            
            slide = cooks.GetComponent<ISlideIn>();
            if (slide == null) throw new Exception("No cook objects!");
            
            disable.DisableOthers(gameObject.name);
            SlideIn();
        }

        protected override IEnumerator SlideInCoroutine()
        {
            yield return base.SlideInCoroutine();
            yield return new WaitForSeconds(0.5f);
            
            cooks.SetActive(true);
            slide.SlideIn();

            inventoryManager.OnClickToggleInventoryPosition();
            
            yield return null;
        }
    }
}