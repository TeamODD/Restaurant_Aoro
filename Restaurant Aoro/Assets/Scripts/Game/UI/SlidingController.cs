using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public interface ISlidable
    {
        public List<Transform> objectTransforms { get; }
        public List<Transform> targetTransforms { get; }
        public float duration { get; } //개별 요소 이동 소요시간
        public Coroutine slidingCoroutine { get; set; }
    }
    
    public interface ISlideIn: ISlidable
    {
        public void SlideIn();
    }
    
    public interface ISlideOut: ISlidable
    {
        public void SlideOut();
    }
    
    public class SlidingController : MonoBehaviour, ISlideIn, ISlideOut
    {
        [SerializeField] private List<Transform> _objectTransforms, _targetTransforms;
        [SerializeField] private float _duration;
        
        public List<Transform> objectTransforms
        {
            get => _objectTransforms;
        }
        public List<Transform> targetTransforms
        {
            get => _targetTransforms;
        }
        public float duration
        {
            get => _duration;
        }
        public Coroutine slidingCoroutine { get; set; }

        public void SlideIn()
        {
            if (slidingCoroutine != null) return;
        
            slidingCoroutine = StartCoroutine(SlideInCoroutine());
        }

        public void SlideOut()
        {
            if (slidingCoroutine != null) return;

            slidingCoroutine = StartCoroutine(SlideOutCoroutine());
        }

        protected virtual IEnumerator SlideInCoroutine()
        {
            yield return new WaitForEndOfFrame(); 
            
            for (var i = 1; i < objectTransforms.Count; i++)
            {
                objectTransforms[i].gameObject.SetActive(true);
                
                var t = 0f;
                var startPos = objectTransforms[i].position;
                var endPos = targetTransforms[i].position;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    var progress = Mathf.Clamp01(t / duration);
                    objectTransforms[i].position = Vector3.Lerp(startPos, endPos, progress);
                    yield return null;
                }
            }
            
            slidingCoroutine = null;
        }

        protected virtual IEnumerator SlideOutCoroutine()
        {
            yield return new WaitForEndOfFrame(); 
            
            for (var i = objectTransforms.Count - 1; i > 0; i--)
            {
                var t = 0f;
                var startPos = targetTransforms[i].position;
                var endPos = targetTransforms[i - 1].position;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    var progress = Mathf.Clamp01(t / duration);
                    objectTransforms[i].position = Vector3.Lerp(startPos, endPos, progress);
                    yield return null;
                }

                objectTransforms[i].gameObject.SetActive(false);
            }

            slidingCoroutine = null;
            gameObject.SetActive(false);
        }
    }
}