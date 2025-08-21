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
            CookManager.instance.PrepareBackground(gameObject);
            SlideIn(true, () => StartCoroutine(slideHelper()));
        }

        private IEnumerator slideHelper()
        {
            yield return new WaitForSeconds(0.5f);

            CookManager.instance.EnterCook();

            yield return null;
        }
    }
}