using UnityEngine;

public class GlobalDialogueClickRelay : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    void Update()
    {
        if (dialogueUI == null || !dialogueUI.IsShowing()) return;
        if (dialogueUI.CurrentMode != DialogueInputMode.Global) return;

        if (Input.GetMouseButtonDown(0)) dialogueUI.AdvanceOrComplete();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            dialogueUI.AdvanceOrComplete();
    }
}
