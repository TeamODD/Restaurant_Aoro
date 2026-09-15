using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour
{
    [SerializeField] private GameObject saveDataPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private LoadGameUIController loadGameUI;

    private void Start()
    {
        var saveFiles = SaveManager.Instance.GetAllSaveFiles();

        foreach (var path in saveFiles)
        {
            GameObject saveData =
                Instantiate(saveDataPrefab, contentTransform);

            string fileName =
                System.IO.Path.GetFileName(path);

            SaveDataSlot saveDataScript =
                saveData.GetComponent<SaveDataSlot>();

            saveDataScript.Initialize(fileName, loadGameUI);
        }
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
