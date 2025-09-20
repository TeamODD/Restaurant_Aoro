using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueClickBlocker : MonoBehaviour, IPointerClickHandler
{
    public DialogueUI target;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (target != null)
            target.OnBlockerClicked();
    }
}
