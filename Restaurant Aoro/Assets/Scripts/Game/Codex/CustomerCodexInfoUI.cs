using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerCodexInfoUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Basic Information")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tribeText;
    [SerializeField] private TMP_Text appearanceTimeText;

    [Header("Preference")]
    [SerializeField] private TMP_Text favoriteTasteText;
    //[SerializeField] private TMP_Text dislikedTasteText;
    [SerializeField] private TMP_Text favoriteFoodText;
    //[SerializeField] private TMP_Text dislikedFoodText;

    [Header("Description")]
    [SerializeField] private TMP_Text basicDescriptionText;
    [SerializeField] private TMP_Text detailDescriptionText;

    [Header("Record")]
    [SerializeField] private TMP_Text visitCountText;
    [SerializeField] private TMP_Text perfectCountText;
    [SerializeField] private TMP_Text excellentCountText;
    [SerializeField] private TMP_Text successCountText;
    [SerializeField] private TMP_Text failCountText;

    [Header("Navigation")]
    [SerializeField] private Button openDetailButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CustomerCodexDetailUI detailUI;
    [SerializeField] private GameObject codexPanel;

    private Customer currentCustomer;
    private CustomerCodexEntry currentEntry;

    private void Awake()
    {
        if (openDetailButton != null)
            openDetailButton.onClick.AddListener(OpenDetail);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        if (openDetailButton != null)
            openDetailButton.onClick.RemoveListener(OpenDetail);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    public void Open(Customer customer, CustomerCodexEntry entry)
    {
        if (customer == null || entry == null)
        {
            Debug.LogWarning(
                "[CustomerCodexInfoUI] Customer 또는 Entry가 null입니다."
            );
            return;
        }

        currentCustomer = customer;
        currentEntry = entry;

        if (root != null)
            root.SetActive(true);

        Refresh();
    }

    private void Refresh()
    {
        if (currentCustomer == null || currentEntry == null)
            return;

        if (nameText != null)
            nameText.text = currentCustomer.CustomerName;

        if (tribeText != null)
            tribeText.text = GetTribeText(currentCustomer.tribe);

        if (appearanceTimeText != null)
        {
            appearanceTimeText.text =
                $"{FormatHour(currentCustomer.appearStartHour)} ~ " +
                $"{FormatHour(currentCustomer.appearEndHour)}";
        }

        bool basicUnlocked =
            currentEntry.basicDescriptionUnlocked;

        bool detailUnlocked =
            currentEntry.detailDescriptionUnlocked;

        if (basicDescriptionText != null)
        {
            basicDescriptionText.text = basicUnlocked
                ? currentCustomer.codexDescription
                : "???";
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = detailUnlocked
                ? currentCustomer.codexDetailDescription
                : "???";
        }

        if (favoriteTasteText != null)
        {
            favoriteTasteText.text = detailUnlocked
                ? JoinValues(currentCustomer.favoriteTastes)
                : "???";
        }
        /*
        if (dislikedTasteText != null)
        {
            dislikedTasteText.text = detailUnlocked
                ? JoinValues(currentCustomer.dislikedTastes)
                : "???";
        }
        */

        if (favoriteFoodText != null)
        {
            favoriteFoodText.text = detailUnlocked
                ? JoinValues(currentCustomer.favoriteFoods)
                : "???";
        }
        /*
        if (dislikedFoodText != null)
        {
            dislikedFoodText.text = detailUnlocked
                ? JoinValues(currentCustomer.dislikedFoods)
                : "???";
        }
        */

        if (visitCountText != null)
            visitCountText.text = currentEntry.visitCount.ToString();

        if (perfectCountText != null)
            perfectCountText.text = currentEntry.perfectCount.ToString();

        if (excellentCountText != null)
            excellentCountText.text = currentEntry.excellentCount.ToString();

        if (successCountText != null)
            successCountText.text = currentEntry.successCount.ToString();

        if (failCountText != null)
            failCountText.text = currentEntry.failCount.ToString();

        if (openDetailButton != null)
        {
            openDetailButton.interactable =
                currentEntry.mainIllustrationUnlocked;
        }
    }

    private void OpenDetail()
    {
        if (currentCustomer == null ||
            currentEntry == null ||
            detailUI == null)
            return;

        if (root != null)
            root.SetActive(false);

        detailUI.Open(currentCustomer, currentEntry);
    }

    public void Close()
    {
        currentCustomer = null;
        currentEntry = null;

        if (root != null)
            root.SetActive(false);

        if (codexPanel != null)
            codexPanel.SetActive(true);
    }

    private string GetTribeText(TribeType tribe)
    {
        return tribe switch
        {
            TribeType.Human => "인간",
            TribeType.Youkai => "요괴",
            _ => "알 수 없음"
        };
    }

    private string FormatHour(float hour)
    {
        int totalMinutes = Mathf.RoundToInt(hour * 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        if (hours >= 24)
            hours = 0;

        return $"{hours:00}:{minutes:00}";
    }

    private string JoinValues<T>(IEnumerable<T> values)
    {
        if (values == null)
            return "없음";

        string result = string.Join(", ", values);

        return string.IsNullOrWhiteSpace(result)
            ? "없음"
            : result;
    }
}