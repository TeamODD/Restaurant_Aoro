using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] panelObjects; // Panel1, 2, 3 (GameObject)
    public ArrowController arrowController;
    public Transform rightArrow;
    public Transform IngredientPanel;
    public Transform FoodPanel;
    public Image IngredientImg;
    public Image FoodImg;
    public InventoryController controller;
    public Camera mainCamera;
    public InventoryUIController uiController;

    public Vector3 offset = new Vector3(10.65f, 0f, 3f);
    public Vector3 offsetCenter = new Vector3(5.45f, 0f, 3f);

    private bool isCentered = false;

    void Start()
    {
        Transform[] panelTransforms = new Transform[panelObjects.Length];
        for (int i = 0; i < panelObjects.Length; i++)
            panelTransforms[i] = panelObjects[i].transform;

        controller.Initialize(mainCamera, panelTransforms, offset);
        controller.SetRightArrow(rightArrow);
    }

    void LateUpdate()
    {
        controller.UpdatePanelPositions(); // 계속 위치 추적
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
            // 원래 위치로 복귀
            controller.MovePanelToCenter(offset, 0.2f);
            isCentered = false;
        }
        else
        {
            // 카메라 중앙으로 이동
            controller.MovePanelToCenter(offsetCenter, 0.2f);
            isCentered = true;
        }
    }
}
