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


    public ReputationState ReputationState;

    private int acceptedCustomer;

    private void OnEnable()
    {
        CustomerManager.OnAnyCustomerAccepted += HandleAccepted;
        ReputationState.OnReputationChanged += UpdateReputationIcon;
    }

    private void OnDisable()
    {
        CustomerManager.OnAnyCustomerAccepted -= HandleAccepted;
        ReputationState.OnReputationChanged -= UpdateReputationIcon;
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
}
