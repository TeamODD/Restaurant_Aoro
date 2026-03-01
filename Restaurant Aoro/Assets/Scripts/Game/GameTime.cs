using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float secondsPerGameMinute = 1f;

    private int hour = 9;
    private int minute = 0;

    private float timer = 0f;

    [SerializeField] private Transform lightParent;
    [SerializeField] private Light2D dayLight;
    [SerializeField] private Gradient dayLightGradient;
    [SerializeField] private Light2D nightLight;
    [SerializeField] private Gradient nightLightGradient;

    void Update()
    {
        // ��� �ð� ����
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
        
        UpdateLight((hour * 60 + minute) / (24f * 60f));
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
}
