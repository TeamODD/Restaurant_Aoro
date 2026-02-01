using UnityEngine;

namespace Scriptable_Object
{
    [CreateAssetMenu(menuName = "Cook/CookRule")]
    public class CookRule: ScriptableObject
    {
        [Header("Ingredient Information")] public Item[] ingredients;
        
        [Header("Food Item")]
        public Item common;
        public Item fine;
        public Item premium;
        public Item exquisite;
    }
}