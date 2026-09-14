using UnityEngine;

public class SaveLoadButtonController : MonoBehaviour
{
    public void OnSaveButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Save] GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.SaveGame();
    }

    public void OnMainButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Save] GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.OnMainButtonClicked();
    }
}
