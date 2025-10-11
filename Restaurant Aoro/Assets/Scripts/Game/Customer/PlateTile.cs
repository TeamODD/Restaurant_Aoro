using Game.Cook;
using UnityEngine;

public class PlateTile : MonoBehaviour
{
    [HideInInspector] public Item item;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Vector2 maxSizeOnPlate = new Vector2(0.8f, 0.8f);
    Vector3 baseScale;

    void Awake()
    {
        baseScale = spriteRenderer.transform.localScale;
    }

    public bool AddItem(Item _item)
    {
        if (_item.ItemType != ItemType.Food) return false;
        if (item) RemoveItem();

        item = _item;
        spriteRenderer.sprite = item.ItemSprite;

        FitSpriteToBox();
        return true;
    }
    void FitSpriteToBox()
    {
        var s = spriteRenderer.sprite;
        if (s == null) return;

        // sprite.bounds는 localScale=1 기준 유니티 단위 크기
        Vector2 size = s.bounds.size;
        if (size.x <= 0f || size.y <= 0f) { spriteRenderer.transform.localScale = baseScale; return; }

        float sx = maxSizeOnPlate.x / size.x;
        float sy = maxSizeOnPlate.y / size.y;
        float scale = Mathf.Min(sx, sy);

        spriteRenderer.transform.localScale = baseScale * scale;
    }
    public void RemoveItem()
    {
        InventoryManager.instance.AddItem(item);
        item = null;
        spriteRenderer.sprite = null;
    }

    private void OnMouseDown()
    {
        if (item) RemoveItem();
        else PlateManager.instance.IngredientAddedToPlateTile(this);
    }
}
