using UnityEngine;
using UnityEngine.UI;

public class TabletButtonController : MonoBehaviour
{
    public TabletState tabletState;
    public SpawnCustomer spawner;

    private Transform seatLocation;

    void Start()
    {
        for (int i = 0; i < tabletState.Seats.Length; i++)
        {
            int index = i;
            Button btn = tabletState.Seats[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SeatButtonClicked(index));
            }
            else
            {
                Debug.LogWarning($"Seat {i}에는 Button 컴포넌트가 없습니다.");
            }
        }
    }

    // 좌석 클릭 함수 (UI 버튼에 연결)
    public void SeatButtonClicked(int seatIndex)
    {
        if (!tabletState.canClicked) return;

        for (int i = 0; i < tabletState.Seats.Length; i++)
        {
            GameObject seat = tabletState.Seats[i];
            SeatState state = seat.GetComponent<SeatState>();
            Image img = seat.GetComponent<Image>();

            if (i == seatIndex && state.isClicked == false)
            {
                state.isClicked = true;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f); // 반투명
                //img.color = Color.red;
            }
            else
            {
                state.isClicked = false;

                if (!state.isSeated)
                    img.color = Color.white;
            }
        }
    }

    public void AcceptButtonClicked()
    {
        if (!tabletState.canClicked) return;

        GameObject currentCustomer = spawner.GetCurrentCustomer();
        foreach (GameObject seat in tabletState.Seats)
        {
            SeatState state = seat.GetComponent<SeatState>();
            Image img = seat.GetComponent<Image>();

            if (state.isClicked)
            {
                state.isSeated = true;
                state.isClicked = false;
                seatLocation = state.SeatLocation;
                img.color = Color.black;
            }
        }
        if (currentCustomer != null)
        {
            var customerManager = currentCustomer.GetComponent<CustomerManager>();
            customerManager.Accept(seatLocation);
        }
        tabletState.canClicked = false;

        bool allSeated = true;
        foreach (GameObject seat in tabletState.Seats)
        {
            SeatState state = seat.GetComponent<SeatState>();
            if (!state.isSeated)
            {
                allSeated = false;
                break;
            }
        }
        tabletState.canSeat = !allSeated;
    }

    public void RefuseButtonClicked()
    {
        if (!tabletState.canClicked) return;

        GameObject currentCustomer = spawner.GetCurrentCustomer();
        foreach (GameObject seat in tabletState.Seats)
        {
            SeatState state = seat.GetComponent<SeatState>();
            Image img = seat.GetComponent<Image>();

            if (!state.isSeated)
            {
                state.isClicked = false;
                img.color = Color.white;
            }
        }
        if (currentCustomer != null)
        {
            var customerManager = currentCustomer.GetComponent<CustomerManager>();
            customerManager.Refuse();
        }
        tabletState.canClicked = false;
    }

    public Transform GetSeatLocation()
    {
        return seatLocation;
    }
}
