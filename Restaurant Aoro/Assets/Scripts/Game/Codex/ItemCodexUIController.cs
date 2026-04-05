using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemCodexUIController : MonoBehaviour
{
    public enum CodexFilterType
    {
        Ingredient,
        Food
    }

    [Header("Filter")]
    public CodexFilterType filterType = CodexFilterType.Ingredient;

    [Header("Slots (8)")]
    public CodexSlotView[] slots;

    [Header("Paging")]
    public Button prevButton;
    public Button nextButton;

    [Header("Locked Visual")]
    public Sprite lockedSprite;

    [Header("Detail Scene")]
    public string detailSceneName = "ItemCodexDetail";

    private List<Item> filteredItems = new();
    private int currentPage = 0;
    private const int pageSize = 8;

    private void OnEnable()
    {
        if (prevButton != null)
        {
            prevButton.onClick.RemoveListener(OnClickPrev);
            prevButton.onClick.AddListener(OnClickPrev);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnClickNext);
            nextButton.onClick.AddListener(OnClickNext);
        }

        ItemCodexManager.OnCodexChanged += RefreshPage;

        BuildFilteredList();
        RefreshPage();
    }

    private void OnDisable()
    {
        if (prevButton != null)
            prevButton.onClick.RemoveListener(OnClickPrev);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnClickNext);

        ItemCodexManager.OnCodexChanged -= RefreshPage;
    }

    private void BuildFilteredList()
    {
        IEnumerable<Item> allItems = ItemDatabase.Instance.GetAllItems();

        if (filterType == CodexFilterType.Ingredient)
            filteredItems = allItems.Where(x => x.ItemType == ItemType.Ingredient).ToList();
        else
            filteredItems = allItems.Where(x => x.ItemType == ItemType.Food).ToList();

        // 필요하면 이름순 정렬
        filteredItems = filteredItems.OrderBy(x => x.ItemName).ToList();
    }

    public void RefreshPage()
    {
        if (filteredItems == null)
            filteredItems = new List<Item>();

        var codexData = ItemCodexManager.Instance.GetAll();

        int totalCount = filteredItems.Count;
        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / pageSize;

        currentPage = Mathf.Clamp(currentPage, 0, maxPage);

        if (prevButton != null)
            prevButton.interactable = currentPage > 0;

        if (nextButton != null)
            nextButton.interactable = currentPage < maxPage;

        int startIndex = currentPage * pageSize;

        for (int i = 0; i < slots.Length; i++)
        {
            int itemIndex = startIndex + i;

            slots[i].gameObject.SetActive(true);

            if (itemIndex >= totalCount)
            {
                slots[i].BindEmpty();
                continue;
                /*slots[i].gameObject.SetActive(false);
                continue;*/
            }

            //slots[i].gameObject.SetActive(true);

            Item item = filteredItems[itemIndex];

            bool unlocked = false;
            if (codexData.TryGetValue(item.ItemID, out var entry))
            {
                unlocked = entry != null && entry.unlocked;
            }

            Sprite displaySprite = unlocked ? item.ItemSprite : lockedSprite;
            string displayName = unlocked ? item.ItemName : "???";

            slots[i].Bind(
                item.ItemID,
                displaySprite,
                displayName,
                unlocked,
                OnClickUnlockedSlot
            );
        }
    }

    private void OnClickPrev()
    {
        if (currentPage <= 0) return;
        currentPage--;
        RefreshPage();
    }

    private void OnClickNext()
    {
        int totalCount = filteredItems.Count;
        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / pageSize;

        if (currentPage >= maxPage) return;
        currentPage++;
        RefreshPage();
    }

    private void OnClickUnlockedSlot(string itemId)
    {
        CodexSelection.SelectedItemId = itemId;
        SceneManager.LoadScene(detailSceneName);
    }
}