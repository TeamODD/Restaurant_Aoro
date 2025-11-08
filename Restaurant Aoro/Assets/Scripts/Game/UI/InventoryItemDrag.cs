using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDrag : DraggingController, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject proxyPrefab;
    private GameObject proxyObj;
    private Item item;

    public void Init(Item _item)
    {
        item = _item;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        if (proxyObj) return;
        proxyObj = Instantiate(proxyPrefab);
        proxyObj.GetComponent<IngredientHitCooktile>().Init(item, gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        if(proxyObj) StartCoroutine(WaitAndDestroy(0.1f));
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        StartCoroutine(WaitAndDestroy(0.1f));
    }

    private IEnumerator WaitAndDestroy(float time)
    { 
        
        yield return new WaitForSeconds(time);
        if (proxyObj) Destroy(proxyObj);
    }

    private void Update()
    {
        if (!isDragging || !proxyObj) return;
        
        var pos = Camera.main.ScreenToWorldPoint(transform.position);
        pos.z = 0;

        proxyObj.transform.position = pos;
    }
}