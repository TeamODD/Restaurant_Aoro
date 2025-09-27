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
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if(obj == null)
        {
            Destroy(gameObject);
            return;
        }
        
        if (other.gameObject.CompareTag("Cook") && !obj.GetComponent<DraggingController>().isDragging)
        {
            var destroy = other.gameObject.GetComponent<CookTile>().AddItem(item);
            if (destroy)
            {
                Destroy(obj.transform.parent.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
