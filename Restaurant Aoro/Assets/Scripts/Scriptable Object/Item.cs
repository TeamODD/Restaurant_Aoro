using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public string ItemType;

    public Sprite ItemSprite;
    
}
