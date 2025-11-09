using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RestaurantManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button openButton;
    public Button closeButton;
    public TMP_Text closeButtonLabel;

    [Header("Exceptional")]
    public GameObject Dialogue;

    [Header("Spawner")]
    public SpawnCustomer spawnCustomer;

    public GameObject Tablet3;
    public TMP_Text VisitingText;
    public TMP_Text CreditText;
    public Image Customer_Icon;
    public Image Youkai_Icon;
    public Button ServeBtn;

    public ReputationState ReputationState;

    private int acceptedCustomer;
    private int currentMoney = 0;
    private bool isOpen = false;
    public bool IsOpen() => isOpen;
    private enum State { Closed, Open, LastOrder }
    private State state = State.Closed;

    private void Start()
    {
        isOpen = false;
        openButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        openButton.onClick.AddListener(OpenRestaurant);
        SetCloseButtonAsLastOrder();
        closeButton.interactable = false;
    }

    private void OnEnable()
    {
        CustomerManager.OnAnyCustomerAccepted += HandleAccepted;
        ReputationState.OnReputationChanged += UpdateReputationIcon;
        PlateTile.AddServeListener(OnServeBtn);
        InventoryController.AddOffServeListener(OffServeBtn);
        CustomerManager.OnMoneyEarned += HandleMoneyEarned;
    }

    private void OnDisable()
    {
        CustomerManager.OnAnyCustomerAccepted -= HandleAccepted;
        ReputationState.OnReputationChanged -= UpdateReputationIcon;
        PlateTile.RemoveServeListener(OnServeBtn);
        InventoryController.RemoveOffServeListener(OffServeBtn);
        CustomerManager.OnMoneyEarned -= HandleMoneyEarned;
    }

    private void OpenRestaurant()
    {
        if (isOpen) return;
        isOpen = true;
        state = State.Open;

        Debug.Log("가게 열림");
        spawnCustomer.StartCustomerFlow();

        openButton.interactable = false;
        SetCloseButtonAsLastOrder();
        closeButton.interactable = true;
    }

    private void LastOrder()
    {
        if (!isOpen || state != State.Open) return;
        state = State.LastOrder;

        Debug.Log("라스트 오더 시작: 새 손님 유입 중단");
        spawnCustomer.StopCustomerFlow();

        SetCloseButtonAsClose();
        closeButton.interactable = true;
    }

    private void CloseRestaurant()
    {
        if (!isOpen) return;
        isOpen = false;
        state = State.Closed;

        Debug.Log("영업 종료: 모든 손님 불만족 퇴장 + 텍스트 초기화");
        spawnCustomer.StopCustomerFlow();

        Dialogue.SetActive(false);
        ForceCloseAllCustomersAsDissatisfied();

        acceptedCustomer = 0;
        currentMoney = 0;
        UpdateAcceptedVisitingText();
        UpdateCreditText();

        openButton.interactable = true;
        SetCloseButtonAsLastOrder();
        closeButton.interactable = false;
    }

    private void SetCloseButtonAsLastOrder()
    {
        if (closeButtonLabel) closeButtonLabel.text = "Last Order";
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(LastOrder);
    }

    private void SetCloseButtonAsClose()
    {
        if (closeButtonLabel) closeButtonLabel.text = "Close";
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseRestaurant);
    }
    private void HandleAccepted()
    {
        acceptedCustomer++;
        UpdateAcceptedVisitingText();
    }

    private void UpdateReputationIcon()
    {
        Customer_Icon.sprite = ReputationState.getCustomerSprite();
        Youkai_Icon.sprite = ReputationState.getYoukaiSprite();
    }

    private void UpdateAcceptedVisitingText()
    {
        VisitingText.text = acceptedCustomer.ToString("D3");
    }

    private void OnServeBtn()
    {
        ServeBtn.gameObject.SetActive(true);
    }

    private void OffServeBtn()
    {
        ServeBtn.gameObject.SetActive(false);
    }

    private void HandleMoneyEarned(int amount)
    {
        AddMoney(amount);
    }

    private void AddMoney(int amount)
    {
        if (amount <= 0) return;
        currentMoney += amount;
        UpdateCreditText();
    }

    private void UpdateCreditText()
    {
        CreditText.text = currentMoney.ToString("N0");
    }

    private void ForceCloseAllCustomersAsDissatisfied()
    {
        var all = FindObjectsOfType<CustomerManager>();
        foreach (var cm in all)
        {
            if (!cm) continue;
            cm.ForceCloseAndLeaveUnhappy();
        }
    }
}
