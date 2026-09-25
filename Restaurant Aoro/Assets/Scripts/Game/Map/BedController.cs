using UnityEngine;
using UnityEngine.UI;
public class BedController : MonoBehaviour
{
    public MapManager mapManager;
    public Slider sleepGauge;
    public int cur_fatigue;

    void Start()
    {
        cur_fatigue = mapManager.Fatigue;
        sleepGauge.minValue = 0f;
        sleepGauge.maxValue = cur_fatigue;
        //sleepGauge.maxValue = 100f;

        sleepGauge.value = 0f;
        sleepGauge.onValueChanged.AddListener(previewSleep);
    }

    public void previewSleep(float value)
    {
        int restore = Mathf.RoundToInt(value);
        int previewFatigue = cur_fatigue - restore;

        Debug.Log($"현재 피로도: {cur_fatigue}"); // UI에 적용
        Debug.Log($"회복량: {restore}");
        Debug.Log($"수면 후 피로도: {previewFatigue}");
    }
    public void sleep()
    {
        int restore = Mathf.RoundToInt(sleepGauge.value);

        mapManager.RestoreFatigue(restore);

        cur_fatigue = mapManager.Fatigue;

    }
}
