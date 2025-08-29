using UnityEngine;

namespace Game.Cook
{
    public class CookTile: MonoBehaviour
    {
        [HideInInspector] public Item item;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void AddItem(Item _item)
        {
            item = _item;
            spriteRenderer.sprite = item.ItemSprite;
        }

        public void RemoveItem()
        {
            item = null;
            spriteRenderer.sprite = null;
        }
    }
}