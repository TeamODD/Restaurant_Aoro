using System;
using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookMenuBtn : SlidingController
    {
        [SerializeField] private DisableOtherObjects disable;

        private void OnMouseDown()
        {
            disable.DisableOthers(gameObject.name);
            SlideIn();
        }

        protected override IEnumerator SlideInCoroutine(bool active, Action onComplete)
        {
            yield return base.SlideInCoroutine(true, null);
            yield return new WaitForSeconds(0.5f);

            CookManager.instance.EnterCook();

            yield return null;
        }
    }
}