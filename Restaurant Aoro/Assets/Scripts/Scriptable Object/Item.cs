using UnityEngine;

public enum ItemType
{
    Ingredient, Food
}

public enum IngredientCategory
{
    Rice, Noodle, Broth, Meat, Seafood, Vegetable, Etc
}

public enum DishMainCategory
{
    Rice, Noodle, Soup, Meat, Seafood, Vegetable
}

public enum DishSubCategory
{
    Etc, Meat, Seafood, Vegetable, None
}

public enum ItemMainCategory
{
    Rice, Noodle, Soup, Meat, Seafood, Vegetable
}

public enum ItemSubCategory
{
    Etc, Meat, Seafood, Vegan, None
}

public enum ItemGrade
{
    Common, Fine, Premium, Exquisite
}


[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public ItemType ItemType;
    public IngredientCategory IngredientCategory;
    public DishMainCategory DishMainCategory;
    public DishSubCategory DishSubCategory;
    public ItemGrade ItemGrade;
    public FoodTaste Foodtaste;
    public Sprite ItemSprite;

    public ItemMainCategory ItemMainCategory => (ItemMainCategory)DishMainCategory;
    public ItemSubCategory ItemSubCategory => (ItemSubCategory)DishSubCategory;
}