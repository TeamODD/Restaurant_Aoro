using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BedController : MonoBehaviour
{
    public MapManager mapManager;
    public Slider sleepSlider;
    public TMP_Text timeText;
    public TMP_Text beforeText;
    public TMP_Text afterText;

    private int cur_fatigue;

    void Start()
    {
        cur_fatigue = mapManager.Fatigue;
        sleepSlider.minValue = 0f;
        sleepSlider.maxValue = 24f;
        //sleepGauge.maxValue = 100f;

        sleepSlider.value = 0f;
        sleepSlider.onValueChanged.AddListener(previewSleep);
    }

    public void previewSleep(float value)
    {
        int sleepTime = Mathf.RoundToInt(value);
        int previewFatigue = Mathf.Max(0, cur_fatigue - (sleepTime * 2));

        timeText.text = sleepTime.ToString() + "time";
        beforeText.text = cur_fatigue.ToString();
        afterText.text = previewFatigue.ToString();

        Debug.Log($"현재 피로도: {cur_fatigue}"); // UI에 적용
        Debug.Log($"수면 시간: {sleepTime}");
        Debug.Log($"수면 후 피로도: {previewFatigue}");
    }
    public void sleep()
    {
        int sleepTime = Mathf.RoundToInt(sleepSlider.value);
        int restore = sleepTime * 2;

        mapManager.RestoreFatigue(restore);

        cur_fatigue = mapManager.Fatigue;
    }
}
