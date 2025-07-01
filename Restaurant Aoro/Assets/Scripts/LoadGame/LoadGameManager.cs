using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour
{
    public GameObject saveDataPrefab;           // SaveData 프리팹
    public Transform contentTransform;          // ScrollView > Viewport > Content

    void Start()
    {
        var saveFiles = SaveManager.Instance.GetAllSaveFiles();

        foreach (var path in saveFiles)
        {
            GameObject saveData = Instantiate(saveDataPrefab, contentTransform);
            string fileName = System.IO.Path.GetFileName(path);

            // 슬롯에 파일명 텍스트 출력
            SaveDataSlot saveDataScript = saveData.GetComponent<SaveDataSlot>();
            saveDataScript.Initialize(fileName);
        }
    }
    public void OnSaveSlotSelected(string fileName)
    {
        SaveManager.Instance.currentSaveFileName = fileName;
        SceneManager.LoadScene("TMPGame");
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
