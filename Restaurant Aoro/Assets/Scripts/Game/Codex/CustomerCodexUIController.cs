using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomerCodexUIController : MonoBehaviour
{
    [Header("Slots (8)")]
    public CodexSlotView[] slots; // Inspector에 8개 할당

    [Header("Paging")]
    public Button prevButton;
    public Button nextButton;

    [Header("Locked Sprite")]
    public Sprite lockedSprite;

    [Header("Detail Scene")]
    public string detailSceneName = "CustomerCodexDetail";

    private List<Customer> allCustomers;
    private int pageIndex = 0;
    private const int PageSize = 8;

    void OnEnable()
    {
        // 전체 목록 캐시
        allCustomers = CustomerDatabase.Instance.GetAll().ToList();

        if (prevButton) prevButton.onClick.AddListener(PrevPage);
        if (nextButton) nextButton.onClick.AddListener(NextPage);

        CustomerCodexManager.OnCodexChanged += Refresh;

        Refresh();
    }

    void OnDisable()
    {
        if (prevButton) prevButton.onClick.RemoveListener(PrevPage);
        if (nextButton) nextButton.onClick.RemoveListener(NextPage);

        CustomerCodexManager.OnCodexChanged -= Refresh;
    }

    public void Refresh()
    {
        var codex = CustomerCodexManager.Instance.GetAll(); // CustomerID -> Entry

        int total = allCustomers.Count;
        int maxPage = Mathf.Max(0, (total - 1) / PageSize);
        pageIndex = Mathf.Clamp(pageIndex, 0, maxPage);

        // 버튼 활성/비활성
        if (prevButton) prevButton.interactable = pageIndex > 0;
        if (nextButton) nextButton.interactable = pageIndex < maxPage;

        int start = pageIndex * PageSize;

        for (int i = 0; i < slots.Length; i++)
        {
            int idx = start + i;

            if (idx >= total)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }

            slots[i].gameObject.SetActive(true);

            var customer = allCustomers[idx];
            codex.TryGetValue(customer.CustomerID, out var entry);

            bool unlocked = entry != null && entry.unlocked;

            // 여기서 "해금된 손님 이미지"는 네 Customer SO에 대표 아이콘이 없으니,
            // 1) Customer에 아이콘 Sprite를 추가하거나
            // 2) tribe/NPCType 기반으로 아이콘 매핑하는 방식이 필요해.
            // 우선은 예시로 null 처리(해금 시 icon을 따로 가져오도록 수정 필요).
            Sprite unlockedIcon = null; // TODO: customer.icon 같은 필드 추천
            string unlockedName = customer.CustomerName;

            slots[i].lockedSprite = lockedSprite;
            slots[i].Bind(customer.CustomerID, unlockedIcon, unlockedName, unlocked, OnClickUnlockedSlot);
        }
    }

    private void PrevPage()
    {
        pageIndex--;
        Refresh();
        // 슬라이드 애니메이션은 여기서 처리하면 됨(아래 4) 참고)
    }

    private void NextPage()
    {
        pageIndex++;
        Refresh();
    }

    private void OnClickUnlockedSlot(string customerId)
    {
        /*CodexSelection.SelectedCustomerId = customerId;
        SceneManager.LoadScene(detailSceneName);*/
    }
}
