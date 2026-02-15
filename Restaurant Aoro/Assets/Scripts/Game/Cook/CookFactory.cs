using System;
using System.Linq;
using Scriptable_Object;
using UnityEngine;

namespace Game.Cook
{
    [Serializable]
    public struct GradePolicy
    {
        public float
            common,
            fine,
            premium,
            exquisite;

        public int
            maxCommon,
            maxFine,
            maxPremium,
            maxExquisite;

        public AnimationCurve qtyWeight;

        public readonly float GetQtyWeight(int qty)
        {
            return qtyWeight.Evaluate(qty);
        }
    }

    public class CookFactory
    {
        private CookRulesSO cookRulesSO = Resources.Load<CookRulesSO>("Cook/CookRules");
        private readonly GradePolicy policy;

        public CookFactory(GradePolicy _policy)
        {
            policy = _policy;
        }

        public Item Make(Item[] ingredients)
        {
            if (cookRulesSO == null) throw new NullReferenceException("[CookFactory] Cook Rule is Empty!");

            var cookRules = cookRulesSO.cookRules;
            cookRules = cookRules.OrderByDescending(rule => rule.ingredients.Count(i => i != null)).ToArray();

            var match = (from cookRule in cookRules
                where cookRule.ingredients.Length <= ingredients.Length
                let commonQty = ingredients.Count(ing => ing != null && ing.ItemGrade == ItemGrade.Common)
                let exquisiteQty = ingredients.Count(ing => ing != null && ing.ItemGrade == ItemGrade.Exquisite)
                let grade = ingredients
                    .Where(ing => ing != null && cookRule.ingredients.Contains(ing))
                    .Select(ing => ing.ItemGrade switch
                    {
                        ItemGrade.Common => policy.common,
                        ItemGrade.Fine => policy.fine,
                        ItemGrade.Premium => policy.premium,
                        ItemGrade.Exquisite => policy.exquisite,
                        _ => throw new Exception("Unknown ItemGrade!")
                    })
                    .Aggregate(1.0, (current, ingGrade) => current * ingGrade) * policy.GetQtyWeight(ingredients.Length)
                select grade switch
                {
                    _ when grade < policy.maxCommon && exquisiteQty == 0 => cookRule.common,
                    _ when grade < policy.maxFine => cookRule.fine,
                    _ when grade < policy.maxPremium && commonQty < 2 => cookRule.premium,
                    _ when grade < policy.maxExquisite && commonQty == 0 && exquisiteQty >= 3 && ingredients.Length >= 4
                        => cookRule.exquisite,
                    _ => throw new Exception("Item grade score exceeds max value!")
                }).FirstOrDefault();

            return match ? match : throw new Exception("Invalid category set detected!");
        }
    }
}