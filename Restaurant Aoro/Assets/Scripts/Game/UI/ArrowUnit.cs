using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite OnMouseImg;
    public Sprite OffMouseImg;
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().sprite = OnMouseImg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().sprite = OffMouseImg;
    }
}
