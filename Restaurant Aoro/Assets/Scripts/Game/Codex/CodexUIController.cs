using UnityEngine;
using UnityEngine.UI;

public class CodexUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject codexRoot;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject ingredientCodexPanel;
    [SerializeField] private GameObject foodCodexPanel;
    [SerializeField] private GameObject recipeCodexPanel;
    [SerializeField] private GameObject customerCodexPanel;
    //[SerializeField] private GameObject savePanel;

    [Header("Buttons")]
    [SerializeField] private Button ingredientCodexButton;
    [SerializeField] private Button foodCodexButton;
    [SerializeField] private Button recipeCodexButton;
    [SerializeField] private Button customerCodexButton;
    [SerializeField] private Button closeButton;
    //[SerializeField] private Button savePanelButton;

    private void Start()
    {
        ingredientCodexButton.onClick.AddListener(OpenIngredientCodex);
        foodCodexButton.onClick.AddListener(OpenFoodCodex);
        recipeCodexButton.onClick.AddListener(OpenRecipeCodex);
        customerCodexButton.onClick.AddListener(OpenCustomerCodex);
        closeButton.onClick.AddListener(CloseCodex);
        //savePanelButton.onClick.AddListener(OpenSavePanel);
    }

    public void OpenIngredientCodex()
    {
        SetActivePanel(ingredientCodexPanel);
    }

    public void OpenFoodCodex()
    {
        SetActivePanel(foodCodexPanel);
    }

    public void OpenRecipeCodex()
    {
        SetActivePanel(recipeCodexPanel);
    }

    public void OpenCustomerCodex()
    {
        SetActivePanel(customerCodexPanel);
    }

    /*
    public void OpenSavePanel()
    {
        SetActivePanel(savePanel);
    }
    */
    public void CloseCodex()
    {
        SetDisActivePanel();
    }

    private void SetActivePanel(GameObject targetPanel)
    {
        Image codexImg = codexRoot.GetComponent<Image>();
        if (codexImg != null)
            codexImg.raycastTarget = true;
        codexPanel.SetActive(true);

        ingredientCodexPanel.SetActive(targetPanel == ingredientCodexPanel);
        foodCodexPanel.SetActive(targetPanel == foodCodexPanel);
        recipeCodexPanel.SetActive(targetPanel == recipeCodexPanel);
        customerCodexPanel.SetActive(targetPanel == customerCodexPanel);
        //savePanel.SetActive(targetPanel == savePanel);
    }

    private void SetDisActivePanel()
    {
        Image codexImg = codexRoot.GetComponent<Image>();
        if (codexImg != null)
            codexImg.raycastTarget = false;
        codexPanel.SetActive(false);
    }
}
