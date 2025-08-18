using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public interface IFadable
    {
        public List<GameObject> objects { get; }
        public float duration { get; }
        public Coroutine fadingCoroutine { get; set; }
    }

    public interface IFadeIn
    {
        public void FadeIn(bool active = true, Action onComplete = null);
    }

    public interface IFadeOut
    {
        public void FadeOut(bool active = true, Action onComplete = null);
    }
    
    public class FadingController: MonoBehaviour, IFadable, IFadeIn, IFadeOut
    {
        public List<GameObject> objects
        {
            get => _objects;
        }
        public float duration
        {
            get => _duration;
        }
        
        public Coroutine fadingCoroutine { get; set; }

        [SerializeField] private List<GameObject> _objects;
        [SerializeField] private float _duration = 2f;
        
        public void FadeIn(bool active = true, Action onComplete = null)
        {
            fadingCoroutine = StartCoroutine(FadeCoroutine(true, active, onComplete));
        }

        public void FadeOut(bool active = true, Action onComplete = null)
        {
            fadingCoroutine = StartCoroutine(FadeCoroutine(false, active, onComplete));
        }

        private IEnumerator FadeCoroutine(bool fadeIn, bool active, Action onComplete)
        {
            yield return new WaitForEndOfFrame();

            foreach (var obj in objects)
            {
                obj.SetActive(true);

                var sprite = obj.GetComponent<SpriteRenderer>();

                var t = 0f;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    var progress = Mathf.Clamp01(t / duration);

                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b,
                        fadeIn ? progress * Time.deltaTime * 255 : 1 - progress * Time.deltaTime * 255);
                    yield return null;
                }
                gameObject.SetActive(active);
            }
            
            onComplete?.Invoke();
            fadingCoroutine = null;
        }
    }
}