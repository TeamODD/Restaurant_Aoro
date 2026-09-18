using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
    public TextMeshProUGUI fileNameText;
    public Button deleteButton;
    public Button clickButton;
    [SerializeField] private LoadGameUIController loadGameUI;
    private string fileName;

    public void Initialize(string name, LoadGameUIController controller)
    {
        fileName = name;
        fileNameText.text = name;
        loadGameUI = controller;

        deleteButton.onClick.AddListener(DeleteThisSave);
        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(OnClickSlot);
    }

    private void DeleteThisSave()
    {
        string path = SaveManager.Instance.GetFullPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[Delete] {fileName}");
        }

        Destroy(gameObject);
    }

    private void OnClickSlot()
    {
        SaveManager.Instance.currentSaveFileName = fileName;

        if (!GameManager.Instance.LoadGameData())
            return;

        loadGameUI.SelectSlot(gameObject);
    }
}
