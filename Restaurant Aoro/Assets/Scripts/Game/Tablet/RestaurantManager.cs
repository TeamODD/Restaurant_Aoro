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

    private void OnEnable()
    {
        CustomerManager.OnAnyCustomerAccepted += HandleAccepted;
        ReputationState.OnReputationChanged += UpdateReputationIcon;
        PlateTile.AddServeListener(OnServeBtn);
        InventoryController.AddOffServeListener(OffServeBtn);
    }

    private void OnDisable()
    {
        CustomerManager.OnAnyCustomerAccepted -= HandleAccepted;
        ReputationState.OnReputationChanged -= UpdateReputationIcon;
        PlateTile.RemoveServeListener(OnServeBtn);
        InventoryController.RemoveOffServeListener(OffServeBtn);
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
}
