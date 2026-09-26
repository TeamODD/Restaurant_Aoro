using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    public GameObject BedPanel;
    public void EnterRestaurant()
    {
        SceneManager.LoadScene("Restaurant 1");
    }
    public void OpenBed()
    {
        BedPanel.SetActive(true);
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
}