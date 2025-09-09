using System;
using Game.Cook;
using UnityEngine;

public class IngredientHitCooktile : MonoBehaviour
{
    private Item item;
    public GameObject obj;
    
    public void Init(Item _item, GameObject _obj)
    {
        obj = _obj;
        item = _item;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Cook"))
        {
            if (item == null) throw new NullReferenceException();
            var destroy = other.gameObject.GetComponent<CookTile>().AddItem(item);
            if(destroy) Destroy(obj.transform.parent.gameObject);
        }
    }
}
