using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public TMP_Text creditText;
    public int CurrentMoney => currentMoney;
    public int Fatigue => GameManager.Instance.Fatigue;
    [SerializeField] private GameTime gameTime;
    private int currentMoney = 0;

    public void SetState(int mvalue, int fvalue)
    {
        currentMoney = Mathf.Max(0, mvalue);
        UpdateCreditText();
    }

    private void UpdateCreditText()
    {
        creditText.text = currentMoney.ToString("N0");
    }

    public void RestoreFatigue(int value)
    {
        GameManager.Instance.RestoreFatigue(value);
    }

    //임시
    public void AddFatigue(int value)
    {
        GameManager.Instance.AddFatigue(value);
    }
}
