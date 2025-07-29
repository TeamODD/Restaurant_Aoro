using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Cooking/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public Sprite icon;
        public string ingredientName;
    }
}