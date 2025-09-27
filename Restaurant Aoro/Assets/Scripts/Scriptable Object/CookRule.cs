using UnityEngine;

namespace Scriptable_Object
{
    [CreateAssetMenu(menuName = "Cook/CookRule")]
    public class CookRule: ScriptableObject
    {
        [Header("Ingredient Information")]
        public ItemMainCategory mainCategory;
        public ItemSubCategory subCategory;
        
        [Header("Food Item")]
        public Item foodPrefab;
    }
}