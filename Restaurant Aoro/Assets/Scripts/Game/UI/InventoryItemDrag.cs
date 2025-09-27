using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDrag : DraggingController, IPointerDownHandler
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
        proxyObj = Instantiate(proxyPrefab);
        proxyObj.GetComponent<IngredientHitCooktile>().Init(item, gameObject);
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
        if (!isDragging) return;
        
        var pos = Camera.main.ScreenToWorldPoint(transform.position);
        pos.z = 0;

        proxyObj.transform.position = pos;
    }
}