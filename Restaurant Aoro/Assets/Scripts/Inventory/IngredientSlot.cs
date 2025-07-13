using Scriptable_Object;

namespace Inventory
{
    public class IngredientSlot
    {
        public Ingredient Ingredient { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => Ingredient == null;

        public bool AddIngredient(Ingredient ingredient, int amount = 1)
        {
            
            return false;
        }
    }
}