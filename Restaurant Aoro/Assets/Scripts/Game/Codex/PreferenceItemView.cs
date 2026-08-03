using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreferenceItemView : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text preferenceText;

    public void Bind(string text)
    {
        if (preferenceText != null)
            preferenceText.text = text;
    }
}