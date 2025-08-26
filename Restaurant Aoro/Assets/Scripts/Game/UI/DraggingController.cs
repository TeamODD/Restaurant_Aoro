using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingController : SlidingController, IDragHandler, IEndDragHandler
{
    public bool isDraggable;

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            objectTransforms.Add(transform);
            targetTransforms.Add(transform.parent.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        SlideIn();
    }
}
