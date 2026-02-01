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

            var cookRules = cookRulesSO.cookRules;
            cookRules = cookRules.OrderByDescending(rule => rule.ingredients.Count(i => i != null)).ToArray();

            Item match = null;
            
            foreach (var cookRule in cookRules)
            {
                if (cookRule.ingredients.Length > ingredients.Length) break;
                
                foreach (var ing in ingredients)
                {
                    if (!cookRule.ingredients.Contains(ing)) break;
                }

                match = cookRule.fine;
            }

            return match ? match : throw new Exception("Invalid category set detected!");
        }
    }
}