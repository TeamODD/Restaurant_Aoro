using UnityEngine;

public class MapTabletController : MonoBehaviour
{
    [SerializeField] private CodexUIController codexUIController;
    [SerializeField] private GameTime time;

    public void TabletOpen()
    {
        time.StopTimer();
    }

    public void TabletClose()
    {
        time.StartTimer();
    }
}
