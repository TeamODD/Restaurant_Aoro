using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomerCodexUIController : MonoBehaviour
{
    [Header("Slots (8)")]
    public CodexSlotView[] slots; // Inspector�� 8�� �Ҵ�

    [Header("Paging")]
    public Button prevButton;
    public Button nextButton;

    private List<Customer> allCustomers = new();
    private int pageIndex = 0;
    private const int PageSize = 8;

    private void OnEnable()
    {
        if (CustomerDatabase.Instance != null)
            allCustomers = CustomerDatabase.Instance.GetAll().ToList();
        else
            allCustomers = new List<Customer>();

        if (prevButton)
        {
            prevButton.onClick.RemoveListener(PrevPage);
            prevButton.onClick.AddListener(PrevPage);
        }

        if (nextButton)
        {
            nextButton.onClick.RemoveListener(NextPage);
            nextButton.onClick.AddListener(NextPage);
        }

        CustomerCodexManager.OnCodexChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (prevButton) prevButton.onClick.RemoveListener(PrevPage);
        if (nextButton) nextButton.onClick.RemoveListener(NextPage);

        CustomerCodexManager.OnCodexChanged -= Refresh;
    }

    public void Refresh()
    {
        if (CustomerCodexManager.Instance == null)
            return;

        var codex = CustomerCodexManager.Instance.GetAll();

        int total = allCustomers.Count;
        int maxPage = total == 0 ? 0 : (total - 1) / PageSize;
        pageIndex = Mathf.Clamp(pageIndex, 0, maxPage);

        if (prevButton) prevButton.interactable = pageIndex > 0;
        if (nextButton) nextButton.interactable = pageIndex < maxPage;

        int start = pageIndex * PageSize;

        for (int i = 0; i < slots.Length; i++)
        {
            int idx = start + i;

            slots[i].gameObject.SetActive(true);

            if (idx >= total)
            {
                slots[i].BindEmpty();
                continue;
            }

            var customer = allCustomers[idx];

            codex.TryGetValue(customer.CustomerID, out var entry);
            bool seen = entry != null && entry.seen;

            Sprite unlockedIcon = null; // 나중에 customer icon 있으면 넣기
            string unlockedName = customer.CustomerName;

            slots[i].Bind(
                customer.CustomerID,
                unlockedIcon,
                unlockedName,
                seen,
                OnClickUnlockedSlot
            );
        }
    }

    private void PrevPage()
    {
        if (pageIndex <= 0) return;
        pageIndex--;
        Refresh();
    }

    private void NextPage()
    {
        int total = allCustomers.Count;
        int maxPage = total == 0 ? 0 : (total - 1) / PageSize;

        if (pageIndex >= maxPage) return;
        pageIndex++;
        Refresh();
    }

    private void OnClickUnlockedSlot(string customerId)
    {
        Debug.Log($"Clicked Customer Codex: {customerId}");
    }
}
