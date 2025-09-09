using Game.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; 
    
    public GameObject[] panelObjects; // Panel1, 2, 3 (GameObject)
    public ArrowController arrowController;
    public RectTransform rightArrow;
    public Transform IngredientPanel;
    public Transform FoodPanel;
    public Image IngredientImg;
    public Image FoodImg;
    public InventoryController controller;
    public Camera mainCamera;
    public InventoryUIController uiController;
    public SlidingController backBtn;

    public Vector2 offset = new Vector2(1065f, 0f); //10.65f, 0f
    public Vector2 offsetCenter = new Vector2(545f, 0f); //5.45f, 0f

    public bool isCentered = false;

    private void Awake()
    {
        if(!instance) instance = this;
    }

    void Start()
    {
        RectTransform[] panelRects = new RectTransform[panelObjects.Length];
        for (int i = 0; i < panelObjects.Length; i++)
            panelRects[i] = panelObjects[i].GetComponent<RectTransform>();

        controller.Initialize(panelRects, offset);
        controller.SetRightArrow(rightArrow);

        foreach (RectTransform rect in panelRects)
            rect.anchoredPosition = offset;
    }

    public void ShowPanel(int index)
    {
        for (int i = 0; i < panelObjects.Length; i++)
            panelObjects[i].SetActive(i == index);
    }

    public void OnClickFoodButton()
    {
        controller.SwapToFoodPanel(IngredientPanel, FoodPanel, IngredientImg, FoodImg);
    }

    public void OnClickIngredientButton()
    {
        controller.SwapToIngredientPanel(IngredientPanel, FoodPanel, IngredientImg, FoodImg);
    }

    public void AddItem(Item item)
    {
        uiController.AddItemToInventory(item);
    }

    public void OnClickToggleInventoryPosition()
    {
        if (isCentered)
        {
            // ���� ��ġ�� ����
            controller.MovePanelToCenter(offset, 0.2f);
            isCentered = false;
        }
        else
        {
            // ī�޶� �߾����� �̵�
            controller.MovePanelToCenter(offsetCenter, 0.2f);
            isCentered = true;
        }
    }

    public void ChangeToInventory()
    {
        panelObjects[0].SetActive(true);
        panelObjects[1].SetActive(false);
        panelObjects[2].SetActive(false);
    }
    public void ChangeToFoodInventory()
    {
        panelObjects[0].SetActive(false);
        panelObjects[1].SetActive(false);
        panelObjects[2].SetActive(true);
    }

    public void ChangeToIngredientInventory()
    {
        panelObjects[0].SetActive(false);
        panelObjects[1].SetActive(true);
        panelObjects[2].SetActive(false);
    }
}
