using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public Image materialIcon;
    public Button clearButton;
    private ItemData assignedItem;

    private Button onClickButton;

    private void Awake()
    {
        onClickButton = GetComponent<Button>();

        clearButton.onClick.AddListener(ClearMaterial);
        clearButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (onClickButton != null)
        {
            onClickButton.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError("UICard에 Button 컴포넌트가 없습니다.");
        }
    }

    public void OnClick()
    {
        CardMaterialManager.Instance.SelectCard(this);
    }

    public void AssignMaterial(ItemData item)
    {
        assignedItem = item;
        materialIcon.sprite = item.icon;
        materialIcon.enabled = true;
        clearButton.gameObject.SetActive(true);
    }

    public void ClearMaterial()
    {
        if (assignedItem == null) return;

        CardMaterialManager.Instance.UnassignFromCard(this, assignedItem);
        assignedItem = null;
        materialIcon.enabled = false;
        clearButton.gameObject.SetActive(false);
    }
}
