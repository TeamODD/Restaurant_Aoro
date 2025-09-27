using UnityEngine;

namespace Scriptable_Object
{
    [CreateAssetMenu(menuName = "Cook/CookRules")]
    public class CookRulesSO : ScriptableObject
    {
        public CookRule[] cookRules;
    }
}