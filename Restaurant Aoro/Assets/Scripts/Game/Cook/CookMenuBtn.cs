using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookMenuBtn : SlidingController
    {
        [SerializeField] private DisableOtherObjects disable;
        [SerializeField] private Transform cookingBar;
        private Vector3 barOrgScale;
        public Transform backgroundPosition;
        private bool cooking;

        private void Start()
        {
            barOrgScale = cookingBar.localScale;
            cookingBar.gameObject.SetActive(false);
        }
        
        public void StartCookingBar(float cookingTime)
        {
            cooking = true;
            cookingBar.gameObject.SetActive(true);
            CookManager.instance.StartCoroutine(ProceedCookingBar(cookingTime));
            CookManager.instance.StartCoroutine(ColorAlpha());
        }

        private IEnumerator ColorAlpha()
        {
            var sr = GetComponent<SpriteRenderer>();
            
            while (cooking)
            {
                cookingBar.GetComponent<SpriteRenderer>().color = sr.color;
                yield return null;
            }
        }

        private IEnumerator ProceedCookingBar(float cookingTime)
        {
            cookingBar.localScale = new Vector3(0, barOrgScale.y, barOrgScale.z);
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(cookingTime / 100);
                cookingBar.localScale = new Vector3(barOrgScale.x * ((float)i / 100), barOrgScale.y, barOrgScale.z);
            }

            cookingBar.gameObject.SetActive(false);
            cooking = false;
        }
        
        private void OnMouseDown()
        {
            if (cooking) return;
            
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