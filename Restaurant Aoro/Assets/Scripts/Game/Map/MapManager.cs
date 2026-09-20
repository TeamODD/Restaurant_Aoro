using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public TMP_Text creditText;
    public int CurrentMoney => currentMoney;
    private int currentMoney = 0;
    public void SetMoney(int value)
    {
        currentMoney = Mathf.Max(0, value);
        UpdateCreditText();
    }

    private void UpdateCreditText()
    {
        creditText.text = currentMoney.ToString("N0");
    }
}
