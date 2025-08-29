using System;
using Game.Cook;
using UnityEngine;

public class IngredientHitCooktile : MonoBehaviour
{
    private Item item;
    
    public void Init(Item _item)
    {
        item = _item;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Cook"))
        {
            if (item == null) throw new NullReferenceException();
            other.gameObject.GetComponent<CookTile>().AddItem(item);
        }
    }
}
