using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public interface IResizable
    {
        public List<GameObject> objects { get; }
        public float duration { get; }
        public Coroutine resizingCoroutine { get; set; }
    }

    public interface IResizeIn
    {
        public void ResizeIn(bool active = true, Action onComplete = null);
    }

    public interface IResizeOut
    {
        public void ResizeOut(bool active = true, Action onComplete = null);
    }
    
    public class ResizingController: MonoBehaviour, IResizable, IResizeIn, IResizeOut
    {
        public List<GameObject> objects
        {
            get => _objects;
        }
        public float duration
        {
            get => _duration;
        }
        
        public Coroutine resizingCoroutine { get; set; }

        [SerializeField] private List<GameObject> _objects;
        [SerializeField] private float _duration = 2f;
        [SerializeField] private List<Vector3> _size;
        private Queue<Vector3> size = new();
        private Queue<Vector3> orgSize = new();
        
        public void ResizeIn(bool active = true, Action onComplete = null)
        {
            foreach (var s in _size)
            {
                size.Enqueue(s);
            }
            foreach (var obj in objects)
            {
                orgSize.Enqueue(obj.transform.localScale);    
            }
            
            resizingCoroutine = StartCoroutine(ResizeCoroutine(size, active, onComplete));
        }

        public void ResizeOut(bool active = true, Action onComplete = null)
        {
            resizingCoroutine = StartCoroutine(ResizeCoroutine(orgSize, active, onComplete));
        }

        private IEnumerator ResizeCoroutine(Queue<Vector3> size, bool active, Action onComplete)
        {
            yield return new WaitForEndOfFrame();

            foreach (var obj in objects)
            {
                obj.SetActive(true);

                var t = 0f;
                var currentSize = transform.localScale;
                var targetSize = size.Dequeue();

                while (t < duration)
                {
                    t += Time.deltaTime;
                    var progress = Mathf.Clamp01(t / duration);

                    transform.localScale = Vector3.Lerp(currentSize, targetSize, progress);
                    
                    yield return null;
                }
                gameObject.SetActive(active);
            }
            
            onComplete?.Invoke();
            resizingCoroutine = null;
        }
    }
}