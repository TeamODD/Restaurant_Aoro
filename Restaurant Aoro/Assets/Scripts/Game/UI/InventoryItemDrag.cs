using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDrag : DraggingController, IPointerDownHandler, IPointerUpHandler
{
    public enum DropTarget { Cook, Plate }

    [SerializeField] private GameObject proxyPrefab;
    [SerializeField] private GameObject plateProxyPrefab;

    [SerializeField] private bool autoSelectByItemType = true;
    [SerializeField] private DropTarget forcedTarget = DropTarget.Cook;
    private GameObject proxyObj;
    private Item item;

    public void Init(Item _item)
    {
        item = _item;
    
    }
    public void UseAutoTargetByItemType(bool useAuto) => autoSelectByItemType = useAuto;
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        if (proxyObj) return;
        DropTarget target = forcedTarget;
        if (autoSelectByItemType && item != null)
            target = (item.ItemType == ItemType.Ingredient) ? DropTarget.Cook : DropTarget.Plate;

        GameObject prefab = (target == DropTarget.Cook) ? proxyPrefab : plateProxyPrefab;
        if (prefab == null)
        {
            isDragging = false;
            return;
        }

        proxyObj = Instantiate(prefab);

        var cook = proxyObj.GetComponent<IngredientHitCooktile>();
        if (cook != null) cook.Init(item, gameObject);

        var plate = proxyObj.GetComponent<FoodHitPlateTile>();
        if (plate != null) plate.Init(item, gameObject);
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