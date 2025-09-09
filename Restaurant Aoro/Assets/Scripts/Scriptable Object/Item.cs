using UnityEngine;

public enum ItemType
{
    Ingredient, Food
}

[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public ItemType ItemType;

    public Sprite ItemSprite;
    
}
