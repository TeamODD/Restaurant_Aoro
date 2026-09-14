using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        clickButton.onClick.RemoveAllListeners(); // �ߺ� ����
        clickButton.onClick.AddListener(OnClickSlot);
    }

    private void DeleteThisSave()
    {
        string path = SaveManager.Instance.GetFullPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[Delete] {fileName} ������");
        }

        Destroy(gameObject); // ������ ����
    }

    private void OnClickSlot()
    {
        SaveManager.Instance.currentSaveFileName = fileName;

        if (!GameManager.Instance.LoadGameData())
            return;

        SceneManager.LoadScene("Restaurant 1");
    }
}
