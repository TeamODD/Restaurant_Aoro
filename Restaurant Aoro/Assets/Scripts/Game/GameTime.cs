using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float secondsPerGameMinute = 1f;

    public int Hour => hour;
    public int Minute => minute;

    private int hour = 9;
    private int minute = 0;

    private float timer = 0f;
    private bool isTired = false;
    private bool isTimerPaused = false;
    private int sleepTimer = 0;

    [SerializeField] private Transform lightParent;
    [SerializeField] private Light2D dayLight;
    [SerializeField] private Gradient dayLightGradient;
    [SerializeField] private Light2D nightLight;
    [SerializeField] private Gradient nightLightGradient;

    public void SetTime(int h, int m)
    {
        hour = Mathf.Clamp(h, 0, 23);
        minute = Mathf.Clamp(m, 0, 59);
        timer = 0f;       // 시간 누적 초기화
        UpdateClockUI();
    }

    void Update()
    {
        if (isTimerPaused)
            return;

        timer += Time.deltaTime;

        if (timer >= secondsPerGameMinute)
        {
            timer = 0f;
            minute++;

            if (minute >= 60)
            {
                minute = 0;
                hour++;

                if (!isTired)
                {
                    sleepTimer++;

                    if (sleepTimer >= 12)
                        isTired = true;
                }
                else
                {
                    GameManager.Instance.AddFatigue(1); // 후에 기절 시스템 추가
                }

            }

            if (hour >= 24)
            {
                hour = 0;
            }

            UpdateClockUI();
            SyncTimeToGameManager();
        }

        UpdateLight((hour * 60 + minute) / (24f * 60f));
    }
    private void SyncTimeToGameManager()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.SetTime(hour, minute);
    }

    void UpdateClockUI()
    {
        timeText.text = string.Format("{0:D2}:{1:D2}", hour, minute);
    }

    private void UpdateLight(float ratio)
    {
        dayLight.color = dayLightGradient.Evaluate(ratio);
        nightLight.color = nightLightGradient.Evaluate(ratio);

        lightParent.rotation = Quaternion.Euler(0, 0, 360.0f * ratio);
    }

    public void StopTimer()
    {
        isTimerPaused = true;
    }

    public void StartTimer()
    {
        isTimerPaused = false;
    }

    public void Sleep(int value)
    {
        if (value <= 0)
            return;
        hour = (hour + value) % 24;

        sleepTimer = 0;
        isTired = false;

        UpdateClockUI();
        SyncTimeToGameManager();

        float ratio = (hour * 60 + minute) / (24f * 60f);
        UpdateLight(ratio);
    }
}
