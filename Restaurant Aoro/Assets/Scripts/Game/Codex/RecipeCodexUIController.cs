using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecipeCodexUIController : MonoBehaviour
{
    [Header("Slots (8)")]
    public CodexSlotView[] slots;

    [Header("Paging")]
    public Button prevButton;
    public Button nextButton;

    [Header("Detail Scene")]
    public string detailSceneName = "RecipeCodexDetail";

    private List<RecipeCodexEntry> recipeEntries = new();
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

        RecipeCodexManager.OnCodexChanged += RefreshPage;

        BuildRecipeList();
        RefreshPage();
    }

    private void OnDisable()
    {
        if (prevButton != null)
            prevButton.onClick.RemoveListener(OnClickPrev);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnClickNext);

        RecipeCodexManager.OnCodexChanged -= RefreshPage;
    }

    private void BuildRecipeList()
    {
        if (RecipeCodexManager.Instance == null)
        {
            recipeEntries = new List<RecipeCodexEntry>();
            return;
        }

        var all = RecipeCodexManager.Instance.GetAll();

        recipeEntries = all.Values
            .OrderBy(x => x.result != null ? x.result.itemName : string.Empty)
            .ToList();
    }

    public void RefreshPage()
    {
        if (RecipeCodexManager.Instance == null)
            return;

        BuildRecipeList();

        int totalCount = recipeEntries.Count;
        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / pageSize;

        currentPage = Mathf.Clamp(currentPage, 0, maxPage);

        if (prevButton != null)
            prevButton.interactable = currentPage > 0;

        if (nextButton != null)
            nextButton.interactable = currentPage < maxPage;

        int startIndex = currentPage * pageSize;

        for (int i = 0; i < slots.Length; i++)
        {
            int recipeIndex = startIndex + i;

            slots[i].gameObject.SetActive(true);

            if (recipeIndex >= totalCount)
            {
                slots[i].BindEmpty();
                continue;
            }

            RecipeCodexEntry entry = recipeEntries[recipeIndex];

            bool unlocked = entry != null && entry.unlocked;

            Sprite displaySprite = null;
            string displayName = "";

            if (unlocked && entry.result != null)
            {
                Item resultItem = ItemDatabase.Instance.GetItem(entry.result.itemId);

                if (resultItem != null)
                {
                    displaySprite = resultItem.ItemSprite;
                    displayName = resultItem.ItemName;
                }
            }

            slots[i].Bind(
                entry.entryId,
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
        int totalCount = recipeEntries.Count;
        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / pageSize;

        if (currentPage >= maxPage) return;
        currentPage++;
        RefreshPage();
    }

    private void OnClickUnlockedSlot(string entryId)
    {
        /*RecipeCodexSelection.SelectedEntryId = entryId;
        SceneManager.LoadScene(detailSceneName);*/
    }
}