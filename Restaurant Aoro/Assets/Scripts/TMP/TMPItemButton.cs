using UnityEngine;

public class TMPItemButton : MonoBehaviour
{
    public Item item;
    public InventoryManager inventoryManager;

    public void OnClick_AddItem()
    {
        inventoryManager.AddItem(item);
    }
}
