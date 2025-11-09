using UnityEngine;

public enum ItemType
{
    Ingredient, Food
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
    public ItemMainCategory ItemMainCategory;
    public ItemSubCategory ItemSubCategory;
    public ItemGrade ItemGrade;
    public FoodTaste Foodtaste;
    public Sprite ItemSprite;
}