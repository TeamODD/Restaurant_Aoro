using UnityEngine;
using UnityEngine.UI;

public class TabletButtonController : MonoBehaviour
{
    public TabletState tabletState;
    public SpawnCustomer spawner;
    public Sprite seat_blank;
    public Sprite seat_filled;
    public Sprite seat_half;

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
                if(state.isSeated == false)
                {
                    state.isClicked = true;
                    tabletState.isTabletClicked = true;
                    img.sprite = seat_half;
                }
                else
                {
                    tabletState.isTabletClicked = false;
                }
            }
            else
            {
                state.isClicked = false;

                if (!state.isSeated)
                    img.sprite = seat_blank;
            }
        }
    }

    public void AcceptButtonClicked()
    {
        if (!tabletState.canClicked) return;
        if (!tabletState.isTabletClicked) return;

        GameObject currentCustomer = spawner.GetCurrentCustomer();
        foreach (GameObject seat in tabletState.Seats)
        {
            SeatState state = seat.GetComponent<SeatState>();
            Image img = seat.GetComponent<Image>();

            if (state.isClicked)
            {
                state.isSeated = true;
                state.isClicked = false;
                seatLocation = state.SeatLocation.transform;
                Debug.Log(seatLocation.position.ToString());
                img.sprite = seat_filled;
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
        tabletState.isTabletClicked = false;
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
                img.sprite = seat_blank;
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
