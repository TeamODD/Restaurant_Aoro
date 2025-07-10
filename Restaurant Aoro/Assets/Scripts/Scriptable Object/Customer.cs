using UnityEngine;

[CreateAssetMenu(menuName ="Customer")]
public class Customer : ScriptableObject
{
    public string CustomerID;
    public string CustomerName;

    public Sprite leftSprite;
    public Sprite frontSprite;
    public Sprite rightSprite;
    public Sprite SeatedSprite;
    public Sprite EatingSprite;
}
