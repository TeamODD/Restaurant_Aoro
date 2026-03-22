using UnityEngine;

namespace Game.Cook
{
    public class CookTile: MonoBehaviour
    {
        [HideInInspector] public Item item;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite itemHoldingSprite;
        [SerializeField] private GameObject deleteButton;
        
        public bool AddItem(Item _item)
        {
            if (_item.ItemType != ItemType.Ingredient) return false;
            if(item) RemoveItem();
            
            item = _item;
            spriteRenderer.sprite = item.ItemSprite;
            return true;
        }

        public void RemoveItemWithoutAdding()
        {
            item = null;
            spriteRenderer.sprite = null;
        }

        public void RemoveItem()
        {
            if(item) InventoryManager.instance.AddItem(item);
            item = null;
            spriteRenderer.sprite = null;
        }

        private void OnMouseEnter()
        {
            if (item == null) return;
            
            deleteButton.SetActive(true);
        }

        private void OnMouseExit()
        { 
            deleteButton.SetActive(false);
        }
        
        private void OnMouseDown()
        {
            if (item)
            {
                RemoveItem();
                GetComponent<SpriteRenderer>().sprite = defaultSprite;
                deleteButton.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().sortingOrder = 9999;
                CookManager.instance.AddIngredientToCookTile(this);
                GetComponent<SpriteRenderer>().sprite = itemHoldingSprite;
            }
        }
    }
}