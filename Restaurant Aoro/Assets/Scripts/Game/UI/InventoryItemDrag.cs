using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDrag : DraggingController, IPointerDownHandler
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

        DropTarget target = forcedTarget;
        if (autoSelectByItemType)
        {
            target = (item.ItemType == ItemType.Ingredient) ? DropTarget.Cook : DropTarget.Plate;
        }

        GameObject prefab = (target == DropTarget.Cook) ? proxyPrefab : plateProxyPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"[InventoryItemDrag] Missing proxy prefab for {target} target.");
            return;
        }

        proxyObj = Instantiate(prefab);

        var cook = proxyObj.GetComponent<IngredientHitCooktile>();
        if (cook != null) cook.Init(item, gameObject);

        var plate = proxyObj.GetComponent<FoodHitPlateTile>();
        if (plate != null) plate.Init(item, gameObject);
        /*isDragging = true;
        proxyObj = Instantiate(proxyPrefab);
        proxyObj.GetComponent<IngredientHitCooktile>().Init(item, gameObject);*/
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