using System;
using Scriptable_Object;
using UnityEngine;

namespace Game.Cook
{
    public class CookFactory
    {
        private CookRulesSO cookRulesSO = Resources.Load<CookRulesSO>("Cook/CookRules");

        public Item Make(ItemMainCategory mainCategory, ItemSubCategory subCategory)
        {
            foreach (var cookRule in cookRulesSO.cookRules)
            {
                if (cookRule.mainCategory == mainCategory && cookRule.subCategory == subCategory)
                {
                    return cookRule.foodPrefab;
                }
            }

            throw new Exception("Invalid category set detected!");
        }
    }
}