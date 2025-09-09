using UnityEngine;

namespace Game.Cook
{
    public class CookTile: MonoBehaviour
    {
        [HideInInspector] public Item item;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public bool AddItem(Item _item)
        {
            if (_item.ItemType != ItemType.Ingredient || item) return false;
            
            item = _item;
            spriteRenderer.sprite = item.ItemSprite;
            return true;
        }

        public void RemoveItem()
        {
            InventoryManager.instance.AddItem(item);
            item = null;
            spriteRenderer.sprite = null;
        }
        
        private void OnMouseDown()
        {
            if(item) RemoveItem();
            else CookManager.instance.IngredientAddedToCookTile(this);
        }
    }
}