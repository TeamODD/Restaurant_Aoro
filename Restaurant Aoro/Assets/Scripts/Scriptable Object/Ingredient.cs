using UnityEngine;

namespace Scriptable_Object
{
    [CreateAssetMenu(fileName = "Ingredient", menuName = "Scriptable Objects/Ingredient")]
    public class Ingredient : ScriptableObject
    {
        [Header("기본 정보")]
        public string ingredientName;
        public Sprite icon;
        public int maxStack = 1;
    }
}
