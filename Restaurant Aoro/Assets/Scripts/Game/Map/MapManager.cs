using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public TMP_Text creditText;
    public int CurrentMoney => currentMoney;
    public int Fatigue => fatigue;
    private int currentMoney = 0;
    private int fatigue;
    public void SetState(int mvalue, int fvalue)
    {
        currentMoney = Mathf.Max(0, mvalue);
        fatigue = Mathf.Max(0, fvalue);
        UpdateCreditText();
    }

    private void UpdateCreditText()
    {
        creditText.text = currentMoney.ToString("N0");
    }

    public void RestoreFatigue(int value)
    {
        fatigue = Mathf.Max(0, fatigue - value);
    }
}
