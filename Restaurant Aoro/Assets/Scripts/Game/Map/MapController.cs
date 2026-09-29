using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private BedController bedController;
    public GameObject BedPanel;
    public void EnterRestaurant()
    {

        SceneManager.LoadScene("Restaurant 1");
    }
    public void OpenBed()
    {
        BedPanel.SetActive(true);
        bedController.bedOpen();
    }
    public void CloseBed()
    {
        BedPanel.SetActive(false);
    }

    public void OpenTablet()
    {

    }

    public void OpenTable()
    {

    }

    //임시
    public void AddFatigue()
    {
        mapManager.AddFatigue(10);
    }
}