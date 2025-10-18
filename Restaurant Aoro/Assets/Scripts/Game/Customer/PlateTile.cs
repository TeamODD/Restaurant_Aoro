using UnityEngine;
using System;

public class PlateTile : MonoBehaviour
{
    [HideInInspector] public Item item;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 maxSizeOnPlate = new Vector2(0.8f, 0.8f);


    Vector3 baseScale;
    Vector3 baseLocalPos;
    BoxCollider2D box;

    public Item PeekItem() => item;

    private static event Action OnServeBtn;
    public static void AddServeListener(Action listener) => OnServeBtn += listener;
    public static void RemoveServeListener(Action listener) => OnServeBtn -= listener;

    void Awake()
    {
        baseScale = spriteRenderer.transform.localScale;
        box = GetComponent<BoxCollider2D>();
        baseLocalPos = spriteRenderer.transform.localPosition;
    }

    public bool AddItem(Item _item)
    {
        if (_item.ItemType != ItemType.Food) return false;
        if (item) RemoveItem();

        item = _item;
        spriteRenderer.sprite = item.ItemSprite;

        FitSpriteToBox();
        AlignSpriteToTileCenter();
        FitColliderToSprite();

        OnServeBtn?.Invoke();
        return true;
    }
    void FitSpriteToBox()
    {
        var s = spriteRenderer.sprite;
        if (s == null) return;

        Vector2 size = s.bounds.size;
        if (size.x <= 0f || size.y <= 0f) { spriteRenderer.transform.localScale = baseScale; return; }

        float sx = maxSizeOnPlate.x / size.x;
        float sy = maxSizeOnPlate.y / size.y;
        float scale = Mathf.Min(sx, sy);

        spriteRenderer.transform.localScale = baseScale * scale;
    }
    void AlignSpriteToTileCenter()
    {
        var sr = spriteRenderer;
        if (sr.sprite == null) return;

        var worldCenter = sr.bounds.center;

        var localCenter = transform.InverseTransformPoint(worldCenter);

        sr.transform.localPosition = baseLocalPos - localCenter;
    }
    void FitColliderToSprite()
    {
        if (box == null || spriteRenderer.sprite == null) return;

        var worldBounds = spriteRenderer.bounds;

        Vector3 localCenter = transform.InverseTransformPoint(worldBounds.center);
        Vector3 lossy = transform.lossyScale;
        Vector2 localSize = new Vector2(
            worldBounds.size.x / Mathf.Max(Mathf.Abs(lossy.x), 1e-6f),
            worldBounds.size.y / Mathf.Max(Mathf.Abs(lossy.y), 1e-6f)
        );

        box.offset = localCenter; 
        box.size = localSize;
    }
    public void RemoveItem()
    {
        if (item == null) return;
        InventoryManager.instance.AddItem(item);
        item = null;
        spriteRenderer.sprite = null;

        spriteRenderer.transform.localScale = baseScale;
        spriteRenderer.transform.localPosition = baseLocalPos;

        InventoryController.InvokeServe();
    }

    private void OnMouseDown()
    {
        if (item) RemoveItem();
        else PlateManager.instance.FoodAddedToPlateTile(this);
    }

    public void ClearSpriteOnly()
    {
        spriteRenderer.sprite = null;
        spriteRenderer.transform.localScale = baseScale;
        spriteRenderer.transform.localPosition = baseLocalPos;

        if (box != null)
        {
            box.size = Vector2.zero;
            box.offset = Vector2.zero;
        }
    }
}
