using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
    public TextMeshProUGUI fileNameText;
    public Button deleteButton;
    public Button clickButton;

    private string fileName;

    public void Initialize(string name)
    {
        fileName = name;
        fileNameText.text = name;

        deleteButton.onClick.AddListener(DeleteThisSave);
        clickButton.onClick.RemoveAllListeners(); // 중복 방지
        clickButton.onClick.AddListener(OnClickSlot);
    }

    private void DeleteThisSave()
    {
        string path = SaveManager.Instance.GetFullPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[Delete] {fileName} 삭제됨");
        }

        Destroy(gameObject); // 프리팹 삭제
    }

    private void OnClickSlot()
    {
        SaveManager.Instance.currentSaveFileName = fileName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("TMPGame");
    }
}
