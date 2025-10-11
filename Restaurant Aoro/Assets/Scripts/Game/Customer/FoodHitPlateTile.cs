using UnityEngine;

public class FoodHitPlateTile : MonoBehaviour
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
        if (other.gameObject.CompareTag("Plate") && !obj.GetComponent<DraggingController>().isDragging)
        {
            var plate = other.gameObject.GetComponent<PlateTile>();
            if (plate != null)
            {
                var added = plate.AddItem(item);
                if (added)
                {
                    Destroy(obj.transform.parent.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
