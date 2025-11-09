using System.Collections;
using Game.UI;
using UnityEngine;

namespace Game.Cook
{
    public class CookMenuBtn : SlidingController
    {
        [SerializeField] private DisableOtherObjects disable;
        [SerializeField] private Transform cookingBar;
        [SerializeField] private Animator cookingIndicator;
        [SerializeField] private Sprite cookingSprite, notCookingSprite;
        private Vector3 barOrgScale;
        public Transform backgroundPosition;
        private bool cooking, cookFinish;
        private SpriteRenderer sr;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            barOrgScale = cookingBar.localScale;
            cookingBar.gameObject.SetActive(false);
            cookingIndicator.gameObject.SetActive(false);
        }

        public void PrepareCook()
        {
            sr.sprite = cookingSprite;
        }

        public void EndCook()
        {
            cookFinish = false;
            sr.sprite = notCookingSprite;
            cookingBar.gameObject.SetActive(false);
            cookingIndicator.gameObject.SetActive(false);
        }

        public void StartCookingBar(float cookingTime)
        {
            cooking = true;
            PrepareCook();
            cookingBar.gameObject.SetActive(true);
            cookingIndicator.gameObject.SetActive(true);
            CookManager.instance.StartCoroutine(ColorAlpha(cookingIndicator.gameObject));
            cookingIndicator.SetBool("Cooking!", true);
            CookManager.instance.StartCoroutine(ProceedCookingBar(cookingTime));
            CookManager.instance.StartCoroutine(ColorAlpha(cookingBar.gameObject));
        }

        private IEnumerator ColorAlpha(GameObject obj)
        {
            var sr = GetComponent<SpriteRenderer>();

            while (cooking)
            {
                obj.GetComponent<SpriteRenderer>().color = sr.color;
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

            FinishCook();
        }

        private void FinishCook()
        {
            //EndCook();
            cookingIndicator.SetBool("Cooking!", false);
            //cookingBar.gameObject.SetActive(false);
            cooking = false;
            cookFinish = true;
        }

        private void OnMouseDown()
        {
            if (cooking) return;
            
            if (cookFinish)
            {
                EndCook();
                return;
            }

            var con = CookManager.instance.PrepareBackground(this);
            if (con) SlideIn(true, () => StartCoroutine(SlideHelper()));
        }

        private IEnumerator SlideHelper()
        {
            yield return new WaitForSeconds(0.5f);

            CookManager.instance.EnterCook();

            yield return null;
        }
    }
}