using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonController : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SaveManager.Instance.currentSaveFileName = SaveManager.Instance.CreateNewSaveFileName();
        SaveManager.Instance.CreateNewSave();

        SceneManager.LoadScene("TMPGame");
    }

    public void OnLoadButtonClicked()
    {
        SceneManager.LoadScene("LoadGame");
    }
}
