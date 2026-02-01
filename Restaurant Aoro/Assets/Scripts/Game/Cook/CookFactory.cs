using System;
using System.Linq;
using Scriptable_Object;
using UnityEngine;

namespace Game.Cook
{
    public class CookFactory
    {
        private CookRulesSO cookRulesSO = Resources.Load<CookRulesSO>("Cook/CookRules");

        public Item Make(Item[] ingredients)
        {
            if (cookRulesSO == null) throw new NullReferenceException("[CookFactory] Cook Rule is Empty!");

            foreach (var cookRule in cookRulesSO.cookRules)
            {
                if (cookRule.ingredients == null || cookRule.ingredients.Length != ingredients.Length)
                    continue;

                var matches = !ingredients.Where((t, i) => t == null || cookRule.ingredients[i] == null || t != cookRule.ingredients[i]).Any();

                if (matches)
                {
                    return cookRule.fine;
                }
            }

            throw new Exception("Invalid category set detected!");
        }
    }
}