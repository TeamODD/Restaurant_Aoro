using System;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingController : SlidingController, IDragHandler, IEndDragHandler
{
    public bool isDraggable;
    public bool isDragging;
    private RectTransform rect;
    
    private void Start()
    {
        isDraggable = false;
        
        for (var i = 0; i < 2; i++)
        {
            objectTransforms.Add(transform);
            targetTransforms.Add(transform.parent.transform);
        }

        rect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rect, 
                eventData.position, 
                eventData.pressEventCamera,
                out var glob))
        {
            rect.position = glob;
        }
    }
    
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        SlideIn(true);
    }
}