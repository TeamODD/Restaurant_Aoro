using System;
using System.Collections.Generic;

[Serializable]
public class RecipeIngredientSnapshot
{
    public string itemId;
    public string itemName;
    public ItemGrade itemGrade;
    public ItemType itemType;
    public ItemMainCategory mainCategory;
    public ItemSubCategory subCategory;
}

[Serializable]
public class RecipeResultSnapshot
{
    public string itemId;
    public string itemName;
    public ItemGrade itemGrade;
    public ItemType itemType;
    public ItemMainCategory mainCategory;
    public ItemSubCategory subCategory;
}

[Serializable]
public class RecipeCodexEntry
{
    public bool unlocked;
    public string entryId;
    public RecipeResultSnapshot result;
    public List<RecipeIngredientSnapshot> ingredients = new();
}