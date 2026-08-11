using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CustomerCodexInfoUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject customerPanel;
    [SerializeField] private GameObject detailPanel;

    [Header("Basic Information")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tribeText;
    [SerializeField] private TMP_Text appearanceTimeText;

    [Header("Preference")]
    [SerializeField] private PreferenceItemView preferenceItemPrefab;

    [SerializeField] private Transform favoritePreferenceContent;

    [Header("Description")]
    //[SerializeField] private TMP_Text basicDescriptionText;
    [SerializeField] private TMP_Text detailDescriptionText;

    [Header("Record")]
    [SerializeField] private TMP_Text visitCountText;
    [SerializeField] private TMP_Text perfectCountText;
    [SerializeField] private TMP_Text excellentCountText;
    [SerializeField] private TMP_Text successCountText;
    [SerializeField] private TMP_Text failCountText;

    [Header("UI Navigation")]
    [SerializeField] private Button openDetailButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CustomerCodexDetailUI detailUI;
    [SerializeField] private GameObject codexPanel;

    [Header("Customer Navigation")]
    [SerializeField] private Button previousCustomerButton;
    [SerializeField] private Button nextCustomerButton;

    [SerializeField] private RectTransform contentRoot;

    [Header("Animation")]
    [SerializeField] private CodexAnimationController animationController;

    private Customer currentCustomer;
    private CustomerCodexEntry currentEntry;
    private CustomerCodexViewState currentViewState;

    private void Awake()
    {
        if (openDetailButton != null)
            openDetailButton.onClick.AddListener(OpenDetail);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        if (openDetailButton != null)
            openDetailButton.onClick.RemoveListener(OpenDetail);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    public void Open(
        Customer customer,
        CustomerCodexEntry entry)
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

        if (customerPanel != null)
            customerPanel.SetActive(true);

        if (animationController != null)
            animationController.ResetToInfo();
        else
        {
            if (infoPanel != null)
                infoPanel.SetActive(true);

            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        ShowInfoState();
        Refresh();
    }

    public void ShowInfoState()
    {
        currentViewState =
            CustomerCodexViewState.Info;

        if (openDetailButton != null)
        {
            openDetailButton.gameObject.SetActive(true);

            openDetailButton.interactable =
                currentEntry != null &&
                currentEntry.mainIllustrationUnlocked;
        }

        if (playButton != null)
            playButton.gameObject.SetActive(false);

        if (closeButton != null)
            closeButton.gameObject.SetActive(true);
    }

    public void ShowDetailState()
    {
        currentViewState =
            CustomerCodexViewState.Detail;

        if (openDetailButton != null)
            openDetailButton.gameObject.SetActive(false);

        if (playButton != null)
            playButton.gameObject.SetActive(true);

        if (closeButton != null)
            closeButton.gameObject.SetActive(true);
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

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = detailUnlocked
                ? currentCustomer.codexDetailDescription
                : "???";
        }


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

        BuildPreferences();
    }

    private void OpenDetail()
    {
        if (currentCustomer == null ||
            currentEntry == null ||
            detailUI == null ||
            animationController == null)
        {
            return;
        }

        if (animationController.IsTransitioning)
            return;

        detailUI.Prepare(currentCustomer, currentEntry);
        animationController.PlayInfoToDetail(ShowDetailState);
    }

    public void Close()
    {
        currentCustomer = null;
        currentEntry = null;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (detailUI != null)
            detailUI.Close();

        if (customerPanel != null)
            customerPanel.SetActive(false);

        if (codexPanel != null)
            codexPanel.SetActive(true);
    }
    private void OnCloseButtonClicked()
    {
        if (animationController != null &&
            animationController.IsTransitioning)
        {
            return;
        }

        switch (currentViewState)
        {
            case CustomerCodexViewState.Info:
                Close();
                break;

            case CustomerCodexViewState.Detail:
                BackToInfo();
                break;
        }
    }
    private void BackToInfo()
    {
        if (animationController == null)
        {
            ShowInfoState();
            return;
        }

        if (animationController.IsTransitioning)
            return;

        if (detailUI != null)
            detailUI.ClearPreviewForTransition();

        animationController.PlayDetailToInfo(
            ShowInfoState
        );
    }

    private string GetTribeText(TribeType tribe)
    {
        return tribe switch
        {
            TribeType.Human => "Human",
            TribeType.Youkai => "Youkai",
            _ => "Unknown"
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

    private void BuildPreferences()
    {
        ClearPreferenceContent(favoritePreferenceContent);

        if (currentCustomer == null || currentEntry == null)
            return;

        if (!currentEntry.detailDescriptionUnlocked)
        {
            CreatePreferenceItem(favoritePreferenceContent, "???");
            return;
        }

        CreatePreferenceItems(
            favoritePreferenceContent,
            currentCustomer.favoriteTastes
        );

        CreatePreferenceItems(
            favoritePreferenceContent,
            currentCustomer.favoriteFoods
        );
    }
    private void CreatePreferenceItems<T>(
    Transform content,
    IEnumerable<T> values)
    {
        if (content == null)
            return;

        bool hasItem = false;

        if (values != null)
        {
            foreach (T value in values)
            {
                CreatePreferenceItem(
                    content,
                    GetDisplayName(value));

                hasItem = true;
            }
        }

        if (!hasItem)
            CreatePreferenceItem(content, "없음");
    }
    private void CreatePreferenceItem(
    Transform content,
    string text)
    {
        if (preferenceItemPrefab == null)
            return;

        PreferenceItemView item =
            Instantiate(preferenceItemPrefab, content);

        item.Bind(text);
    }
    private void ClearPreferenceContent(
    Transform content)
    {
        if (content == null)
            return;

        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
    private string GetDisplayName<T>(T value)
    {
        return value switch
        {
            FoodTaste.Sweet => "Sweet", //단맛
            FoodTaste.Salty => "Salty", //짠맛
            FoodTaste.Spicy => "Spicy", //매운맛
            FoodTaste.Sour => "Sour", //신맛
            FoodTaste.Bitter => "Bitter", //쓴맛

            _ => value?.ToString() ?? ""
        };
    }

    public void Prepare(
        Customer customer,
        CustomerCodexEntry entry)
    {
        if (customer == null || entry == null)
            return;

        currentCustomer = customer;
        currentEntry = entry;

        Refresh();
    }
    private List<Customer> GetUnlockedCustomers()
    {
        if (CustomerDatabase.Instance == null ||
            CustomerCodexManager.Instance == null)
            return new List<Customer>();

        Dictionary<string, CustomerCodexEntry> codex =
            CustomerCodexManager.Instance.GetAll();

        return CustomerDatabase.Instance
            .GetAll()
            .Where(customer =>
            {
                return codex.TryGetValue(
                        customer.CustomerID,
                        out CustomerCodexEntry entry
                    )
                    && entry.seen;
            })
            .ToList();
    }

    private void NextCustomer()
    {
        if (animationController == null ||
            animationController.IsTransitioning)
            return;

        List<Customer> unlockedCustomers = GetUnlockedCustomers();

        int currentIndex = unlockedCustomers.FindIndex(
            customer =>
                customer.CustomerID == currentCustomer.CustomerID
            );
        if (currentIndex < 0 || currentIndex >= unlockedCustomers.Count - 1)
            return;

        Customer nextCustomer =
            unlockedCustomers[currentIndex + 1];

        animationController.PlaySlideNext(
            contentRoot,
            () => ChangeCustomer(nextCustomer)
        );
    }
    private void PreviousCustomer()
    {
        if (animationController == null ||
            animationController.IsTransitioning)
            return;

        List<Customer> unlockedCustomers =
            GetUnlockedCustomers();

        int currentIndex =
            unlockedCustomers.FindIndex(
                customer =>
                    customer.CustomerID ==
                    currentCustomer.CustomerID
            );

        if (currentIndex <= 0)
            return;

        Customer previousCustomer =
            unlockedCustomers[currentIndex - 1];

        animationController.PlaySlidePrevious(
            contentRoot,
            () => ChangeCustomer(previousCustomer)
        );
    }

    private void ChangeCustomer(Customer customer)
    {
        if (customer == null ||
            CustomerCodexManager.Instance == null)
            return;

        Dictionary<string, CustomerCodexEntry> codex =
            CustomerCodexManager.Instance.GetAll();

        if (!codex.TryGetValue(
                customer.CustomerID,
                out CustomerCodexEntry entry))
            return;

        currentCustomer = customer;
        currentEntry = entry;

        Refresh();
        UpdateNavigationButtons();
    }
    private void UpdateNavigationButtons()
    {
        List<Customer> unlockedCustomers =
            GetUnlockedCustomers();

        int currentIndex =
            unlockedCustomers.FindIndex(
                customer =>
                    customer.CustomerID ==
                    currentCustomer.CustomerID
            );

        if (previousCustomerButton != null)
        {
            previousCustomerButton.interactable =
                currentIndex > 0;
        }

        if (nextCustomerButton != null)
        {
            nextCustomerButton.interactable =
                currentIndex >= 0 &&
                currentIndex < unlockedCustomers.Count - 1;
        }
    }
}