using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float secondsPerGameMinute = 1f;

    private int hour = 9;
    private int minute = 0;

    private float timer = 0f;

    void Update()
    {
        // 경과 시간 누적
        timer += Time.deltaTime;

        if (timer >= secondsPerGameMinute)
        {
            timer = 0f;
            minute++;

            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }

            if (hour >= 24)
            {
                hour = 0;
            }

            UpdateClockUI();
        }
    }

    void UpdateClockUI()
    {
        timeText.text = string.Format("{0:D2}:{1:D2}", hour, minute);
    }
}
