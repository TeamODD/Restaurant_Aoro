using UnityEngine;

public class TabletState : MonoBehaviour
{
    public GameObject[] Seats;
    public bool isTabletClicked;
    public bool canClicked;
    public bool canSeat;

    void Start()
    {
        canClicked = false;
        canSeat = true;
        isTabletClicked = false;
    }
}
