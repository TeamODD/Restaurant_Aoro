using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public void EnterRestaurant()
    {
        SceneManager.LoadScene("Restaurant 1");
    }
}