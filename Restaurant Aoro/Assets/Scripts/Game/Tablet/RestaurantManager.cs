using UnityEngine;
using TMPro;

public class RestaurantManager : MonoBehaviour
{
    public GameObject Tablet3;
    public TMP_Text VisitingText;
    public TMP_Text CreditText;
    public TMP_Text RequtationText;

    private int acceptedCustomer;

    private void OnEnable()
    {
        CustomerManager.OnAnyCustomerAccepted += HandleAccepted;
    }

    private void OnDisable()
    {
        CustomerManager.OnAnyCustomerAccepted -= HandleAccepted;
    }

    private void HandleAccepted()
    {
        acceptedCustomer++;
        UpdateAcceptedVisitingText();
    }

    private void UpdateAcceptedVisitingText()
    {
        VisitingText.text = acceptedCustomer.ToString("D3");
    }
}
