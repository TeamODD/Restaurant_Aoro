using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookMenuBtn : SlidingController
    {
        [SerializeField] private DisableOtherObjects disable;
        public Transform backgroundPosition;

        private void OnMouseDown()
        {
            var con = CookManager.instance.PrepareBackground(this);
            if(con) SlideIn(true, () => StartCoroutine(SlideHelper()));
        }

        private IEnumerator SlideHelper()
        {
            yield return new WaitForSeconds(0.5f);

            CookManager.instance.EnterCook();

            yield return null;
        }
    }
}