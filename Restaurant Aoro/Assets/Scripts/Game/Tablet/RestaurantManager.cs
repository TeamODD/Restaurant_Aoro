using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RestaurantManager : MonoBehaviour
{
    public GameObject Tablet3;
    public TMP_Text VisitingText;
    public TMP_Text CreditText;
    public Image Customer_Icon;
    public Image Youkai_Icon;
    public Button ServeBtn;

    public ReputationState ReputationState;

    private int acceptedCustomer;

    private int currentMoney = 0;

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
}
